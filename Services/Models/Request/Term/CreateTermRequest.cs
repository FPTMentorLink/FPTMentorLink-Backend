using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Services.Utils;
using System.Text;

namespace Services.Models.Request.Term;

public class CreateTermRequest : IValidatableObject
{
    [Required] [MaxLength(255)] public string Code { get; set; } = null!;

    [Required] public DateTime StartTime { get; set; }

    [Required] public DateTime EndTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errorBuilder = new StringBuilder();

        if (StartTime > EndTime)
        {
            errorBuilder.Append("Start time must be before end time; ");
        }

        if (StartTime.Date < DateTime.UtcNow.Date)
        {
            errorBuilder.Append("Start time cannot be in the past; ");
        }

        if (errorBuilder.Length > 0)
        {
            yield return Helper.CreateValidationResult(errorBuilder.ToString());
        }
    }
}