namespace Services.DTOs;

public class FeedbackDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Content { get; set; } = null!;
    public int Rate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateFeedbackDto
{
    public Guid AppointmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Content { get; set; } = null!;
    public int Rate { get; set; }
}

public class UpdateFeedbackDto
{
    public string? Content { get; set; }
    public int? Rate { get; set; }
} 