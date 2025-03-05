using Services.Models.Request.Base;

namespace Services.Models.Request.Checkpoint;

public class GetCheckpointsRequest : PaginationQuery
{
    public Guid TermId { get; set; }
}