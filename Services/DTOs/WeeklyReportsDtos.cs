namespace Services.DTOs;

public class WeeklyReportsDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWeeklyReportsDto
{
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class UpdateWeeklyReportsDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
} 