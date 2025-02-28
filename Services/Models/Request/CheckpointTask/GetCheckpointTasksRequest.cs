using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.CheckpointTask;

public class GetCheckpointTasksRequest : PaginationQuery
{
    public Guid? CheckpointId { get; set; }
    public Guid? ProjectId { get; set; }
    public CheckpointTaskStatus? Status { get; set; }
}