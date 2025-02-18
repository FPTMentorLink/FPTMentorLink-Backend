namespace Services.DTOs;

public class CheckpointDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TermId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCheckpointDto
{
    public string Name { get; set; } = null!;
    public Guid TermId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UpdateCheckpointDto
{
    public string? Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}