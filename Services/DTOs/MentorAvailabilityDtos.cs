namespace Services.DTOs;

public class MentorAvailabilityDto
{
    public Guid Id { get; set; }
    public Guid MentorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMentorAvailabilityDto
{
    public Guid MentorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class UpdateMentorAvailabilityDto
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 