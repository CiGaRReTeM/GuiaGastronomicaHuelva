using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Api.Services;
using GuiaGastronomica.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuiaGastronomica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(AppDbContext context, ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("reset-database")]
    public async Task<ActionResult> ResetDatabase()
    {
        try
        {
            _logger.LogWarning("🔴 INICIANDO LIMPIEZA DE BASE DE DATOS");

            // Eliminar todos los reviews primero (por constraint de clave foránea)
            var reviewCount = await _context.Reviews.CountAsync();
            await _context.Reviews.ExecuteDeleteAsync();
            _logger.LogInformation($"Eliminadas {reviewCount} reseñas");

            // Eliminar todos los venues
            var venueCount = await _context.Venues.CountAsync();
            await _context.Venues.ExecuteDeleteAsync();
            _logger.LogInformation($"Eliminados {venueCount} locales");

            // Eliminar todas las zonas
            var zoneCount = await _context.Zones.CountAsync();
            await _context.Zones.ExecuteDeleteAsync();
            _logger.LogInformation($"Eliminadas {zoneCount} zonas");

            return Ok(new 
            { 
                message = "Base de datos limpiada correctamente",
                deletedReviews = reviewCount,
                deletedVenues = venueCount,
                deletedZones = zoneCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al limpiar BD: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("initialize-zones")]
    public async Task<ActionResult> InitializeZones()
    {
        try
        {
            _logger.LogInformation("Inicializando 17 zonas de Huelva capital...");

            var zones = new List<Zone>
            {
                new Zone { Name = "Nuevo Parque, Los Rosales, Tráfico Pesado" },
                new Zone { Name = "Marismas del Polvorín" },
                new Zone { Name = "Pescadería" },
                new Zone { Name = "Centro" },
                new Zone { Name = "Reina Victoria, Matadero" },
                new Zone { Name = "Isla Chica" },
                new Zone { Name = "Las Torres, Guadalupe" },
                new Zone { Name = "Fuentepiña" },
                new Zone { Name = "La Florida, Vistalegre" },
                new Zone { Name = "La Hispanidad, Verdeluz" },
                new Zone { Name = "Tres Ventanas" },
                new Zone { Name = "El Conquero" },
                new Zone { Name = "Molino de la Vega" },
                new Zone { Name = "Las Colonias" },
                new Zone { Name = "El Carmen, Cardeñas" },
                new Zone { Name = "La Orden" },
                new Zone { Name = "San Antonio" }
            };

            await _context.Zones.AddRangeAsync(zones);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"✅ {zones.Count} zonas creadas correctamente");

            return Ok(new 
            { 
                message = "Zonas inicializadas",
                zonesCreated = zones.Count,
                zones = zones.Select(z => z.Name).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al inicializar zonas: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("assign-venues-to-zones")]
    public async Task<ActionResult> AssignVenuesToZones([FromServices] ZoneAssignmentService zoneService)
    {
        try
        {
            _logger.LogInformation("🔄 Iniciando asignación de venues a zonas...");

            // Obtener todos los venues
            var venues = await _context.Venues.ToListAsync();
            _logger.LogInformation($"Total de venues a procesar: {venues.Count}");

            // Asignar zonas usando el servicio
            int assignedCount = zoneService.AssignZonesToVenues(venues);
            
            // Actualizar en BD
            foreach (var venue in venues)
            {
                var assignedZone = zoneService.AssignZone(venue);
                venue.Zone = assignedZone;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation($"✅ Asignación completada: {assignedCount} venues asignados a zonas específicas");

            // Obtener estadísticas
            var distribution = zoneService.GetZoneDistribution(venues);

            return Ok(new 
            { 
                message = "Asignación de zonas completada",
                totalProcessed = venues.Count,
                assigned = assignedCount,
                distribution = distribution
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en asignación de zonas: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("database-status")]
    public async Task<ActionResult> GetDatabaseStatus()
    {
        try
        {
            var venueCount = await _context.Venues.CountAsync();
            var reviewCount = await _context.Reviews.CountAsync();
            var zoneCount = await _context.Zones.CountAsync();

            var zones = await _context.Zones
                .Select(z => new { z.Id, z.Name })
                .ToListAsync();

            return Ok(new 
            { 
                venues = venueCount,
                reviews = reviewCount,
                zones = zoneCount,
                zonesList = zones
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener estado: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }
}
