namespace Services.Models.Request.Checkpoint;

public class UpdateCheckpointRequest
{
    public string? Name { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}