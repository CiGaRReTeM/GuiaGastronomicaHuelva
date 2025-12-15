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

            // Buscar contexto relevante en la base de datos
            var venuesContext = await GetRelevantVenuesContext(userMessage);
            _logger.LogInformation("Found {Count} venues in context", venuesContext.Count);

            // Construir mensaje con contexto de venues
            var contextMessage = BuildVenuesContext(venuesContext);
            
            // Agregar mensaje del usuario con contexto incorporado
            var enrichedMessage = $"{contextMessage}\n\nPregunta del usuario: {userMessage}";
            _chatHistory.AddUserMessage(enrichedMessage);

            _logger.LogInformation("Calling Ollama via Semantic Kernel...");

            // Obtener servicio de chat completion de Semantic Kernel
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();

            // Generar respuesta con Semantic Kernel
            var response = await chatService.GetChatMessageContentAsync(
                _chatHistory,
                kernel: _kernel
            );

            _logger.LogInformation("Received response from Ollama, length: {Length}", response.Content?.Length ?? 0);

            var responseText = response.Content ?? "Lo siento, no pude generar una respuesta.";

            // Agregar respuesta del asistente al historial
            _chatHistory.AddAssistantMessage(responseText);

            return new ChatResponseDto
            {
                Response = responseText,
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

    private string BuildVenuesContext(List<VenueDto> venuesContext)
    {
        var contextBuilder = new System.Text.StringBuilder();
        
        contextBuilder.AppendLine("Contexto: Eres un asistente experto en gastronomía de Huelva, España.");
        
        if (!venuesContext.Any())
        {
            contextBuilder.AppendLine("Actualmente no hay locales en la base de datos que coincidan con la búsqueda.");
            return contextBuilder.ToString();
        }

        contextBuilder.AppendLine($"Tienes información sobre {venuesContext.Count} locales:");
        contextBuilder.AppendLine();

        foreach (var venue in venuesContext.Take(10))
        {
            contextBuilder.AppendLine($"• {venue.Name}");
            contextBuilder.AppendLine($"  Tipo: {venue.Category}");
            contextBuilder.AppendLine($"  Zona: {venue.Zone}");
            contextBuilder.AppendLine($"  Valoración: {venue.Score}/5 estrellas");
            if (!string.IsNullOrEmpty(venue.Description))
            {
                contextBuilder.AppendLine($"  Info: {venue.Description}");
            }
            contextBuilder.AppendLine();
        }

        contextBuilder.AppendLine("Importante: Solo recomienda estos locales. Responde de forma amigable en español.");
        return contextBuilder.ToString();
    }
}
