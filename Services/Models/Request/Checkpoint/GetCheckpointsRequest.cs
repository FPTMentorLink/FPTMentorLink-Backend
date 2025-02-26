using Services.Utils;

namespace Services.Models.Request.Checkpoint;

public class GetCheckpointsRequest : PaginationParams
{
    public Guid TermId { get; set; }
}