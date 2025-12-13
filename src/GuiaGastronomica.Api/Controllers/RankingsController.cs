using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GuiaGastronomica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RankingsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<RankingsController> _logger;

    public RankingsController(AppDbContext context, ILogger<RankingsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<RankingDto>> GetRankings([FromQuery] int top = 50)
    {
        _logger.LogInformation("Getting rankings - Top: {Top}", top);

        var ranking = new RankingDto();

        // Ranking global
        ranking.GlobalRanking = await _context.Venues
            .Where(v => v.IsActive)
            .OrderByDescending(v => v.Score)
            .Take(top)
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

        // Ranking por zonas
        var zones = await _context.Venues
            .Where(v => v.IsActive && !string.IsNullOrEmpty(v.Zone))
            .Select(v => v.Zone!)
            .Distinct()
            .ToListAsync();

        foreach (var zone in zones)
        {
            var zoneVenues = await _context.Venues
                .Where(v => v.IsActive && v.Zone == zone)
                .OrderByDescending(v => v.Score)
                .Take(top)
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

            ranking.RankingByZone[zone] = zoneVenues;
        }

        return Ok(ranking);
    }
}
