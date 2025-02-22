using Repositories.Entities;

namespace Services.Models.Request.CheckpointTask;

public class CreateCheckpointTaskRequest
{
    public Guid CheckpointId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public double? Score { get; set; }
    public CheckpointTaskStatus Status { get; set; }
}