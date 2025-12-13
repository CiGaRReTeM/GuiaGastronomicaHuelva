using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Shared.DTOs;
using GuiaGastronomica.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace GuiaGastronomica.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(AppDbContext context, ILogger<ReviewsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> PostReview([FromBody] ReviewDto reviewDto)
    {
        _logger.LogInformation("Received review for venue {VenueId}", reviewDto.VenueId);

        var venue = await _context.Venues.FindAsync(reviewDto.VenueId);
        if (venue == null)
            return NotFound("Venue not found");

        var review = new Review
        {
            VenueId = reviewDto.VenueId,
            Content = reviewDto.Content,
            Rating = reviewDto.Rating,
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Review submitted successfully", reviewId = review.Id });
    }
}
