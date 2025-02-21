namespace Services.Models.Request.Checkpoint;

public class CreateCheckpointRequest
{
    public string Name { get; set; } = null!;
    public Guid TermId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}