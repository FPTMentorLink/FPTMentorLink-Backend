namespace Services.DTOs;

public class WeeklyReportDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWeeklyReportDto
{
    public Guid ProjectId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class UpdateWeeklyReportDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
} 