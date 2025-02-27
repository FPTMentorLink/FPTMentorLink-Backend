using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.CheckpointTask;

public class UpdateCheckpointTaskStatus : ValidatorObject
{
    public CheckpointTaskStatus? Status { get; set; }
}