using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Shared.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace GuiaGastronomica.Api.Services;

public class ChatService
{
    private readonly Kernel _kernel;
    private readonly AppDbContext _context;
    private readonly ILogger<ChatService> _logger;
    private readonly ChatHistory _chatHistory;

    public ChatService(Kernel kernel, AppDbContext context, ILogger<ChatService> logger)
    {
        _kernel = kernel;
        _context = context;
        _logger = logger;
        _chatHistory = new ChatHistory();
        
        // Inicializar el sistema con el contexto del proyecto
        _chatHistory.AddSystemMessage(@"Eres un asistente experto en restaurantes y gastronomía de Huelva, España.
Tu objetivo es ayudar a los usuarios a encontrar los mejores locales según sus preferencias.
Puedes recomendar restaurantes, bares, chiringuitos, tabernas y otros establecimientos gastronómicos.
Responde de forma amigable, clara y concisa en español.
Cuando recomiendes un lugar, menciona su nombre, zona y puntuación si está disponible.");
    }

    public async Task<ChatResponseDto> ProcessMessageAsync(string userMessage)
    {
        try
        {
            _logger.LogInformation("Processing chat message: {Message}", userMessage);

            // Agregar mensaje del usuario al historial
            _chatHistory.AddUserMessage(userMessage);

            // Buscar contexto relevante en la base de datos
            var venuesContext = await GetRelevantVenuesContext(userMessage);

            // Llamar directamente a Ollama API
            var response = await CallOllamaAsync(userMessage, venuesContext.Cast<object>().ToList());

            // Agregar respuesta del asistente al historial
            _chatHistory.AddAssistantMessage(response);

            return new ChatResponseDto
            {
                Response = response,
                ExtractedIntent = ExtractIntent(userMessage),
                ExtractedEntities = new Dictionary<string, object>
                {
                    { "venues_context", venuesContext }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat message: {ErrorMessage}", ex.Message);
            return new ChatResponseDto
            {
                Response = $"Error: {ex.Message}. InnerException: {ex.InnerException?.Message ?? "ninguna"}",
                ExtractedIntent = "error"
            };
        }
    }

    private async Task<List<VenueDto>> GetRelevantVenuesContext(string message)
    {
        // Búsqueda simple por palabras clave
        var keywords = new[] { "restaurante", "bar", "taberna", "chiringuito", "cervecería", "mesón" };
        var zones = new[] { "centro", "molinos", "isla cristina", "punta umbría", "rompido" };

        var query = _context.Venues.Where(v => v.IsActive);

        // Filtrar por zona si se menciona
        var messageLower = message.ToLower();
        var mentionedZone = zones.FirstOrDefault(z => messageLower.Contains(z));
        if (mentionedZone != null)
        {
            query = query.Where(v => v.Zone!.ToLower().Contains(mentionedZone));
        }

        // Obtener top 5 venues por puntuación
        var venues = await query
            .OrderByDescending(v => v.Score)
            .Take(5)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                Description = v.Description,
                Category = v.Category,
                Zone = v.Zone,
                Score = v.Score,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                ReviewCount = _context.Reviews.Count(r => r.VenueId == v.Id)
            })
            .ToListAsync();

        return venues;
    }

    private string ExtractIntent(string message)
    {
        var messageLower = message.ToLower();

        if (messageLower.Contains("recomendar") || messageLower.Contains("sugerir") || messageLower.Contains("mejor"))
            return "recommendation";
        
        if (messageLower.Contains("donde") || messageLower.Contains("dónde") || messageLower.Contains("ubicación"))
            return "location";
        
        if (messageLower.Contains("precio") || messageLower.Contains("caro") || messageLower.Contains("barato"))
            return "price";
        
        if (messageLower.Contains("horario") || messageLower.Contains("abierto"))
            return "hours";

        return "general";
    }

    public void ClearHistory()
    {
        _chatHistory.Clear();
        _chatHistory.AddSystemMessage(@"Eres un asistente experto en restaurantes y gastronomía de Huelva, España.
Tu objetivo es ayudar a los usuarios a encontrar los mejores locales según sus preferencias.");
    }

    private async Task<string> CallOllamaAsync(string userMessage, List<object> venuesContext)
    {
        using var httpClient = new HttpClient();
        
        // Construir el prompt con contexto
        var systemPrompt = @"Eres un asistente experto en restaurantes y gastronomía de Huelva, España.

IMPORTANTE: Solo puedes recomendar locales que existan en la base de datos proporcionada.
NO inventes ni menciones lugares que no estén en la lista.
Si no hay locales relevantes, indícalo claramente.

Responde de forma amigable, clara y concisa en español.";

        var contextInfo = "";
        if (venuesContext.Any())
        {
            contextInfo = "\n\n=== LOCALES DISPONIBLES EN HUELVA ===\n";
            foreach (var venue in venuesContext.Take(10))
            {
                var v = venue as VenueDto;
                if (v != null)
                {
                    contextInfo += $"- {v.Name} ({v.Category}) en {v.Zone}. Puntuación: {v.Score}/5\n";
                    if (!string.IsNullOrEmpty(v.Description))
                        contextInfo += $"  {v.Description}\n";
                }
            }
            contextInfo += "=================================\n";
        }
        else
        {
            contextInfo = "\n\n=== NO HAY LOCALES EN LA BASE DE DATOS AÚN ===\n";
        }

        var fullPrompt = $"{systemPrompt}{contextInfo}\nUsuario: {userMessage}\nAsistente:";

        var requestBody = new
        {
            model = "llama3.2:3b",
            prompt = fullPrompt,
            stream = false
        };

        var response = await httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate", 
            requestBody
        );

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
        return result?.response ?? "Lo siento, no pude generar una respuesta.";
    }

    private class OllamaResponse
    {
        public string? response { get; set; }
    }
}
