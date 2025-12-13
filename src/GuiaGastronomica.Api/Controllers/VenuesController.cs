using GuiaGastronomica.Api.Data;
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

    public VenuesController(AppDbContext context, ILogger<VenuesController> logger)
    {
        _context = context;
        _logger = logger;
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
}
