namespace Services.DTOs;

public class CheckpointDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCheckpointDto
{
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateCheckpointDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
} 