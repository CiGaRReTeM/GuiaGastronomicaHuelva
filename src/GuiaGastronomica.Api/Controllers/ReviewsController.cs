using GuiaGastronomica.Api.Data;
using GuiaGastronomica.Shared.DTOs;
using GuiaGastronomica.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet("venue/{venueId}")]
    public async Task<ActionResult<List<ReviewDto>>> GetVenueReviews(int venueId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        _logger.LogInformation($"Getting reviews for venue {venueId}");

        var reviews = await _context.Reviews
            .Where(r => r.VenueId == venueId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                VenueId = r.VenueId,
                Content = r.Content,
                Rating = r.Rating,
                Sentiment = r.Sentiment,
                IsVerified = r.IsVerified,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReview(int id)
    {
        var review = await _context.Reviews
            .Where(r => r.Id == id)
            .Select(r => new ReviewDto
            {
                Id = r.Id,
                VenueId = r.VenueId,
                Content = r.Content,
                Rating = r.Rating,
                Sentiment = r.Sentiment,
                IsVerified = r.IsVerified,
                CreatedAt = r.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (review == null)
            return NotFound();

        return Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewRequest request)
    {
        try
        {
            _logger.LogInformation($"Creating review for venue {request.VenueId}");

            // Validar que el venue existe
            var venue = await _context.Venues.FindAsync(request.VenueId);
            if (venue == null)
                return BadRequest(new { error = "Local no encontrado" });

            // Validar campos
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "La reseña no puede estar vacía" });

            if (request.Rating < 1 || request.Rating > 5)
                return BadRequest(new { error = "La puntuación debe estar entre 1 y 5" });

            // Crear reseña
            var review = new Review
            {
                VenueId = request.VenueId,
                Content = request.Content,
                Rating = request.Rating,
                Sentiment = AnalyzeSentiment(request.Content),
                IsVerified = false, // Requiere moderación
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Review created with ID {review.Id}");

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, new ReviewDto
            {
                Id = review.Id,
                VenueId = review.VenueId,
                Content = review.Content,
                Rating = review.Rating,
                Sentiment = review.Sentiment,
                IsVerified = review.IsVerified,
                CreatedAt = review.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating review: {ex.Message}");
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("stats/venue/{venueId}")]
    public async Task<ActionResult<ReviewStatsDto>> GetVenueReviewStats(int venueId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.VenueId == venueId && r.IsVerified)
            .ToListAsync();

        if (reviews.Count == 0)
            return Ok(new ReviewStatsDto
            {
                TotalReviews = 0,
                AverageRating = 0.0,
                SentimentDistribution = new SentimentDistributionDto { Positive = 0, Neutral = 0, Negative = 0 }
            });

        var averageRating = reviews.Average(r => r.Rating);
        var sentimentDist = new SentimentDistributionDto
        {
            Positive = reviews.Count(r => r.Sentiment == "positive"),
            Neutral = reviews.Count(r => r.Sentiment == "neutral"),
            Negative = reviews.Count(r => r.Sentiment == "negative")
        };

        return Ok(new ReviewStatsDto
        {
            TotalReviews = reviews.Count,
            AverageRating = Math.Round(averageRating, 1),
            SentimentDistribution = sentimentDist
        });
    }

    private string AnalyzeSentiment(string text)
    {
        // Simple sentiment analysis basado en palabras clave
        var positiveWords = new[] { "excelente", "muy bueno", "delicioso", "recomiendo", "perfecto", "increíble", "fantástico", "maravilloso", "buenísimo" };
        var negativeWords = new[] { "malo", "horrible", "desastre", "no recomiendo", "pésimo", "terrible", "decepcionante", "mediocre" };

        var lowerText = text.ToLower();

        var positiveCount = positiveWords.Count(word => lowerText.Contains(word));
        var negativeCount = negativeWords.Count(word => lowerText.Contains(word));

        if (positiveCount > negativeCount)
            return "positive";
        else if (negativeCount > positiveCount)
            return "negative";
        else
            return "neutral";
    }
}

public class CreateReviewRequest
{
    public int VenueId { get; set; }
    public required string Content { get; set; }
    public int Rating { get; set; } // 1-5
}
