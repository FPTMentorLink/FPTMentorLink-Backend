using Repositories.Entities;

namespace Services.Models.Response.CheckpointTask;

public class CheckpointTaskResponse
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