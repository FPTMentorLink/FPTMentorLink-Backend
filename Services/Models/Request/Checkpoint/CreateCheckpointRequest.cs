using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Checkpoint;

public class CreateCheckpointRequest : Command
{
    [Required] public string Name { get; set; } = null!;
    [Required] public Guid TermId { get; set; }
    [Required] public DateTime StartTime { get; set; }
    [Required] public DateTime EndTime { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime > EndTime)
        {
            yield return Helper.CreateValidationResult("Start time must be before end time");
        }

        if (StartTime.Date < DateTime.UtcNow.Date)
        {
            yield return Helper.CreateValidationResult("Start time cannot be in the past");
        }
    }
}