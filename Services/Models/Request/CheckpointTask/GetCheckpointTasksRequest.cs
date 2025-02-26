using Repositories.Entities;
using Services.Utils;

namespace Services.Models.Request.CheckpointTask;

public class GetCheckpointTasksRequest : PaginationParams
{
    public Guid? CheckpointId { get; set; }
    public Guid? ProjectId { get; set; }
    public CheckpointTaskStatus? Status { get; set; }
}