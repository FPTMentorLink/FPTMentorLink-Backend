using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.CheckpointTask;

public class UpdateCheckpointTaskStatus : Command
{
    public CheckpointTaskStatus? Status { get; set; }
}