using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Checkpoint;

public class GetCheckpointsRequest : PaginationQuery
{
    public Guid TermId { get; set; }
}