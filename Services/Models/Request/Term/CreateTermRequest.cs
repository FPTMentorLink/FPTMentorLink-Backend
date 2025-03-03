using System.ComponentModel.DataAnnotations;
using Services.Utils;
using Services.Models.Request.Base;

namespace Services.Models.Request.Term;

public class CreateTermRequest : ValidatableObject
{
    [Required] [MaxLength(255)] public string Code { get; set; } = null!;

    [Required] public DateTime StartTime { get; set; }

    [Required] public DateTime EndTime { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime > EndTime)
        {
            yield return Helper.CreateValidationResult("Start time must be before end time; ");
        }

        if (StartTime.Date < DateTime.UtcNow.Date)
        {
            yield return Helper.CreateValidationResult("Start time cannot be in the past; ");
        }
    }
}