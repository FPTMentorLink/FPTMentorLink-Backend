using Repositories.Entities;

namespace Services.Models.Request.CheckpointTask;

public class UpdateCheckpointTaskStatus
{
    public CheckpointTaskStatus? Status { get; set; }
}