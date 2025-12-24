using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Api.Services;
using GuiaGastronomica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuiaGastronomica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<VenuesController> _logger;
    private readonly GooglePlacesService _googlePlacesService;

    public VenuesController(AppDbContext context, ILogger<VenuesController> logger, GooglePlacesService googlePlacesService)
    {
        _context = context;
        _logger = logger;
        _googlePlacesService = googlePlacesService;
    }

    [HttpGet]
    public async Task<ActionResult<List<VenueDto>>> GetVenues(
        [FromQuery] string? zone = null,
        [FromQuery] string? category = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        _logger.LogInformation("Getting venues - Zone: {Zone}, Category: {Category}, Page: {Page}", 
            zone, category, page);

        var query = _context.Venues.Where(v => v.IsActive);

        if (!string.IsNullOrEmpty(zone))
            query = query.Where(v => v.Zone == zone);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(v => v.Category == category);

        var venues = await query
            .OrderByDescending(v => v.Score)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                Description = v.Description,
                Category = v.Category,
                Zone = v.Zone,
                PriceRange = v.PriceRange,
                Score = v.Score,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                ReviewCount = _context.Reviews.Count(r => r.VenueId == v.Id)
            })
            .ToListAsync();

        return Ok(venues);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VenueDto>> GetVenue(int id)
    {
        var venue = await _context.Venues
            .Where(v => v.Id == id && v.IsActive)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                Description = v.Description,
                Category = v.Category,
                Zone = v.Zone,
                PriceRange = v.PriceRange,
                Score = v.Score,
                Latitude = v.Latitude,
                Longitude = v.Longitude,
                ReviewCount = _context.Reviews.Count(r => r.VenueId == v.Id)
            })
            .FirstOrDefaultAsync();

        if (venue == null)
            return NotFound();

        return Ok(venue);
    }

    [HttpPost("import-from-google-places")]
    public async Task<ActionResult> ImportFromGooglePlaces()
    {
        try
        {
            _logger.LogInformation("Iniciando importación de Google Places...");

            // Obtener venues de Google Places
            var venues = await _googlePlacesService.SearchHuelvaVenuesAsync();
            _logger.LogInformation($"Se encontraron {venues.Count} locales en Google Places");

            if (venues.Count == 0)
                return Ok(new { message = "No se encontraron locales en Google Places" });

            // Deduplicación: verificar si ya existen
            var newVenues = new List<GuiaGastronomica.Shared.Models.Venue>();
            var duplicateCount = 0;

            foreach (var venue in venues)
            {
                // Buscar por nombre y zona (para evitar duplicados)
                var existing = await _context.Venues
                    .FirstOrDefaultAsync(v => v.Name.ToLower() == venue.Name.ToLower() && v.Zone == venue.Zone);

                if (existing == null)
                {
                    newVenues.Add(venue);
                }
                else
                {
                    duplicateCount++;
                }
            }

            _logger.LogInformation($"Locales nuevos: {newVenues.Count}, Duplicados: {duplicateCount}");

            // Agregar a base de datos
            if (newVenues.Count > 0)
            {
                await _context.Venues.AddRangeAsync(newVenues);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Importación completada. {newVenues.Count} locales agregados.");
            }

            return Ok(new 
            { 
                message = "Importación completada", 
                totalFound = venues.Count,
                newAdded = newVenues.Count,
                duplicates = duplicateCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error en importación: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }
}
