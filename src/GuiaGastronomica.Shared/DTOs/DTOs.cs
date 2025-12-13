namespace GuiaGastronomica.Shared.DTOs;

public class VenueDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Zone { get; set; }
    public decimal? PriceRange { get; set; }
    public double Score { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int ReviewCount { get; set; }
}

public class ReviewDto
{
    public required string Content { get; set; }
    public int Rating { get; set; }
    public int VenueId { get; set; }
}

public class RankingDto
{
    public List<VenueDto> GlobalRanking { get; set; } = new();
    public Dictionary<string, List<VenueDto>> RankingByZone { get; set; } = new();
}

public class ChatMessageDto
{
    public required string Message { get; set; }
    public string? VenueContext { get; set; }
}

public class ChatResponseDto
{
    public required string Response { get; set; }
    public string? ExtractedIntent { get; set; }
    public Dictionary<string, object>? ExtractedEntities { get; set; }
}
