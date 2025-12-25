using System.Text.Json;
using GuiaGastronomica.Shared.Models;
using Microsoft.Extensions.Logging;

namespace GuiaGastronomica.Api.Services;

public class GooglePlacesService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GooglePlacesService> _logger;
    private readonly string _apiKey;

    // Coordenadas aproximadas de Huelva (ciudad)
    private const double HuelvaLatitude = 37.2574;
    private const double HuelvaLongitude = -6.9501;
    private const int SearchRadiusMeters = 15000; // 15 km de radio

    public GooglePlacesService(HttpClient httpClient, IConfiguration configuration, ILogger<GooglePlacesService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["GooglePlaces:ApiKey"];
    }

    /// <summary>
    /// Busca todos los restaurantes y bares en Huelva usando Google Places API (Text Search)
    /// </summary>
    public async Task<List<Venue>> SearchHuelvaVenuesAsync()
    {
        if (string.IsNullOrEmpty(_apiKey))
            throw new InvalidOperationException("GooglePlaces:ApiKey no está configurada. Configura la variable de entorno GooglePlaces__ApiKey o el appsettings.json");

        var venues = new List<Venue>();

        try
        {
            // Búsqueda por texto es más efectiva que Nearby Search
            var queries = new[] 
            { 
                "restaurantes en Huelva España",
                "bares en Huelva España",
                "cafeterías en Huelva España",
                "tapas Huelva"
            };

            foreach (var query in queries)
            {
                _logger.LogInformation($"Buscando: {query}...");
                var queryVenues = await SearchByTextAsync(query);
                venues.AddRange(queryVenues);
            }

            // Deduplicación por nombre
            var uniqueVenues = venues
                .GroupBy(v => v.Name.ToLower())
                .Select(g => g.First())
                .ToList();

            _logger.LogInformation($"Total de locales únicos encontrados: {uniqueVenues.Count}");
            return uniqueVenues;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al buscar venues en Google Places: {ex.Message}");
            throw;
        }
    }

    private async Task<List<Venue>> SearchByTextAsync(string query)
    {
        var venues = new List<Venue>();
        var pageToken = "";
        var requestCount = 0;

        do
        {
            requestCount++;
            _logger.LogInformation($"Solicitud {requestCount} para búsqueda: '{query}'");

            var url = BuildTextSearchUrl(query, pageToken);
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error en Google Places API: {response.StatusCode}");
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error detail: {errorContent}");
                break;
            }

            var content = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(content);
            var root = jsonDoc.RootElement;

            // Verificar status
            if (root.TryGetProperty("status", out var statusElement))
            {
                var status = statusElement.GetString();
                _logger.LogInformation($"Google Places API Status: {status}");
                
                if (status != "OK" && status != "ZERO_RESULTS")
                {
                    _logger.LogWarning($"API Status no es OK: {status}");
                    break;
                }
            }

            // Procesar resultados
            if (root.TryGetProperty("results", out var results))
            {
                _logger.LogInformation($"Resultados en esta página: {results.GetArrayLength()}");
                
                foreach (var result in results.EnumerateArray())
                {
                    var venue = ParseGooglePlacesResult(result, "restaurant");
                    if (venue != null)
                    {
                        venues.Add(venue);
                    }
                }
            }

            // Obtener siguiente página
            pageToken = root.TryGetProperty("next_page_token", out var token) 
                ? token.GetString() ?? "" 
                : "";

            if (!string.IsNullOrEmpty(pageToken))
            {
                await Task.Delay(2000);
            }
        } while (!string.IsNullOrEmpty(pageToken));

        return venues;
    }

    private string BuildTextSearchUrl(string query, string pageToken = "")
    {
        var baseUrl = "https://maps.googleapis.com/maps/api/place/textsearch/json";
        var encodedQuery = Uri.EscapeDataString(query);

        var url = $"{baseUrl}?query={encodedQuery}&key={_apiKey}";

        if (!string.IsNullOrEmpty(pageToken))
        {
            url += $"&pagetoken={pageToken}";
        }

        return url;
    }

    private Venue? ParseGooglePlacesResult(JsonElement result, string type)
    {
        try
        {
            // Campos obligatorios
            if (!result.TryGetProperty("name", out var nameElement) ||
                !result.TryGetProperty("geometry", out var geometryElement))
            {
                return null;
            }

            var name = nameElement.GetString();
            if (string.IsNullOrWhiteSpace(name)) return null;

            // Coordenadas
            if (!geometryElement.TryGetProperty("location", out var locationElement) ||
                !locationElement.TryGetProperty("lat", out var latElement) ||
                !locationElement.TryGetProperty("lng", out var lngElement))
            {
                return null;
            }

            var latitude = latElement.GetDouble();
            var longitude = lngElement.GetDouble();

            // Campos opcionales
            var address = result.TryGetProperty("vicinity", out var addressElement) 
                ? addressElement.GetString() ?? "Dirección no disponible"
                : "Dirección no disponible";

            var phone = result.TryGetProperty("formatted_phone_number", out var phoneElement)
                ? phoneElement.GetString()
                : null;

            var website = result.TryGetProperty("website", out var websiteElement)
                ? websiteElement.GetString()
                : null;

            var rating = result.TryGetProperty("rating", out var ratingElement)
                ? ratingElement.GetDouble()
                : 0.0;

            var venue = new Venue
            {
                Name = name,
                Address = address,
                Phone = phone,
                Website = website,
                Latitude = latitude,
                Longitude = longitude,
                Category = NormalizeCategory(type),
                Zone = "Otras", // Asignación manual después
                Score = rating,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Description = $"Importado desde Google Places"
            };

            return venue;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Error al parsear resultado de Google Places: {ex.Message}");
            return null;
        }
    }

    private string NormalizeCategory(string googleType)
    {
        return googleType switch
        {
            "restaurant" => "Restaurante",
            "cafe" => "Café",
            "bar" => "Bar",
            "tapas" => "Tapería",
            "bistro" => "Bistró",
            "pizzeria" => "Pizzería",
            "panaderia" => "Panadería",
            _ => "Establecimiento"
        };
    }
}
