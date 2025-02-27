using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Term;

public class UpdateTermRequest : Command
{
    [MaxLength(255)] public string? Code { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime.HasValue && EndTime.HasValue && StartTime >= EndTime)
        {
            yield return Helper.CreateValidationResult("Start time must be before end time");
        }
        if (StartTime.HasValue && StartTime.Value.Date < DateTime.UtcNow.Date)
        {
            yield return Helper.CreateValidationResult("Start time cannot be in the past");
        }
    }
}