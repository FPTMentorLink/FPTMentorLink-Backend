using Services.Models.Response.Base;

namespace Services.Models.Response.Checkpoint;

public class CheckpointResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid TermId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}