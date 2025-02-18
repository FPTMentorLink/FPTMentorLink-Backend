using Repositories.Entities;

namespace Services.DTOs;

public class CheckpointTaskDto
{
    public Guid Id { get; set; }
    public Guid CheckpointId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CheckpointTaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateCheckpointTaskDto
{
    public Guid CheckpointId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public CheckpointTaskStatus Status { get; set; }
}

public class UpdateCheckpointTaskDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
} 

public class UpdateCheckpointTaskStatus
{
    public CheckpointTaskStatus? Status { get; set; }
}