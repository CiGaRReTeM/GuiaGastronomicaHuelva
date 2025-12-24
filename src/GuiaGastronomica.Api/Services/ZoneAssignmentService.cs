using GuiaGastronomica.Shared.Models;

namespace GuiaGastronomica.Api.Services;

/// <summary>
/// Servicio para asignar restaurantes a zonas basado en geolocalización
/// </summary>
public class ZoneAssignmentService
{
    private readonly ILogger<ZoneAssignmentService> _logger;

    /// <summary>
    /// Puntos de referencia centrales para cada zona de Huelva
    /// Coordenadas aproximadas del centro geográfico de cada barrio
    /// </summary>
    private readonly Dictionary<string, (double latitude, double longitude)> _zoneCoordinates = new()
    {
        // Zona 1: Nuevo Parque, Los Rosales, Tráfico Pesado
        { "Nuevo Parque, Los Rosales, Tráfico Pesado", (37.2610, -6.9380) },
        
        // Zona 2: Marismas del Polvorín
        { "Marismas del Polvorín", (37.2540, -6.9450) },
        
        // Zona 3: Pescadería
        { "Pescadería", (37.2550, -6.9400) },
        
        // Zona 4: Centro (Casco Viejo, Plaza de las Monjas, Plaza de la Merced)
        { "Centro", (37.2571, -6.9406) },
        
        // Zona 5: Reina Victoria, Matadero
        { "Reina Victoria, Matadero", (37.2620, -6.9350) },
        
        // Zona 6: Isla Chica
        { "Isla Chica", (37.2530, -6.9380) },
        
        // Zona 7: Las Torres, Guadalupe
        { "Las Torres, Guadalupe", (37.2680, -6.9300) },
        
        // Zona 8: Fuentepiña
        { "Fuentepiña", (37.2700, -6.9250) },
        
        // Zona 9: La Florida, Vistalegre
        { "La Florida, Vistalegre", (37.2750, -6.9200) },
        
        // Zona 10: La Hispanidad, Verdeluz
        { "La Hispanidad, Verdeluz", (37.2700, -6.9400) },
        
        // Zona 11: Tres Ventanas
        { "Tres Ventanas", (37.2650, -6.9450) },
        
        // Zona 12: El Conquero
        { "El Conquero", (37.2600, -6.9500) },
        
        // Zona 13: Molino de la Vega
        { "Molino de la Vega", (37.2580, -6.9320) },
        
        // Zona 14: Las Colonias
        { "Las Colonias", (37.2630, -6.9250) },
        
        // Zona 15: El Carmen, Cardeñas
        { "El Carmen, Cardeñas", (37.2550, -6.9320) },
        
        // Zona 16: La Orden
        { "La Orden", (37.2500, -6.9350) },
        
        // Zona 17: San Antonio
        { "San Antonio", (37.2520, -6.9500) }
    };

    /// <summary>
    /// Radio de búsqueda en kilómetros para asignar venues a zonas
    /// </summary>
    private const double SearchRadiusKm = 2.0;

    public ZoneAssignmentService(ILogger<ZoneAssignmentService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calcula la distancia en kilómetros entre dos puntos geográficos (Haversine formula)
    /// </summary>
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Radio de la Tierra en km

        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = R * c;

        return distance;
    }

    /// <summary>
    /// Asigna un venue a la zona más cercana basándose en coordenadas
    /// </summary>
    public string AssignZone(Venue venue)
    {
        if (venue.Latitude == 0 || venue.Longitude == 0)
        {
            _logger.LogWarning($"Venue {venue.Name} sin coordenadas válidas, se asigna a 'Otras'");
            return "Otras";
        }

        double minDistance = double.MaxValue;
        string? closestZone = null;

        foreach (var zoneCoord in _zoneCoordinates)
        {
            var distance = CalculateDistance(
                venue.Latitude, venue.Longitude,
                zoneCoord.Value.latitude, zoneCoord.Value.longitude
            );

            if (distance < minDistance)
            {
                minDistance = distance;
                closestZone = zoneCoord.Key;
            }
        }

        // Si la zona más cercana está a menos de SearchRadiusKm, la asignamos
        if (minDistance <= SearchRadiusKm && closestZone != null)
        {
            _logger.LogInformation($"✓ {venue.Name} → {closestZone} (distancia: {minDistance:F2} km)");
            return closestZone;
        }

        // Si está demasiado lejos, asignar a "Otras"
        _logger.LogWarning($"⚠ {venue.Name} demasiado lejos de cualquier zona ({minDistance:F2} km), se asigna a 'Otras'");
        return "Otras";
    }

    /// <summary>
    /// Asigna zonas a múltiples venues
    /// </summary>
    public int AssignZonesToVenues(IEnumerable<Venue> venues)
    {
        int assignedCount = 0;

        foreach (var venue in venues)
        {
            var assignedZone = AssignZone(venue);
            if (assignedZone != "Otras")
            {
                assignedCount++;
            }
        }

        return assignedCount;
    }

    /// <summary>
    /// Obtiene estadísticas de distribución de venues por zona
    /// </summary>
    public Dictionary<string, int> GetZoneDistribution(IEnumerable<Venue> venues)
    {
        var distribution = new Dictionary<string, int>();

        foreach (var zone in _zoneCoordinates.Keys)
        {
            distribution[zone] = 0;
        }
        distribution["Otras"] = 0;

        foreach (var venue in venues)
        {
            var zone = venue.Zone ?? "Otras";
            if (distribution.ContainsKey(zone))
            {
                distribution[zone]++;
            }
            else
            {
                distribution["Otras"]++;
            }
        }

        return distribution;
    }
}
