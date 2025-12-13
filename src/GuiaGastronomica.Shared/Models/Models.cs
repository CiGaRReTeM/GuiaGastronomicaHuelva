namespace GuiaGastronomica.Shared.Models;

public class Venue
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Category { get; set; }
    public string? Zone { get; set; }
    public decimal? PriceRange { get; set; }
    public double Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Review
{
    public int Id { get; set; }
    public int VenueId { get; set; }
    public int? UserId { get; set; }
    public required string Content { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Sentiment { get; set; } // positive, negative, neutral
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserFeedback
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int? VenueId { get; set; }
    public required string Message { get; set; }
    public string? ExtractedData { get; set; } // JSON
    public string? Sentiment { get; set; }
    public bool Verified { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Zone
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? GeoJson { get; set; } // Geometría del barrio/distrito
}
