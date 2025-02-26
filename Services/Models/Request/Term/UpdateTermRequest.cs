using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using System.Text;
using Services.Utils;

namespace Services.Models.Request.Term;

public class UpdateTermRequest : IValidatableObject
{
    [MaxLength(255)] public string? Code { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errorBuilder = new StringBuilder();

        if (StartTime.HasValue && EndTime.HasValue && StartTime >= EndTime)
        {
            errorBuilder.Append("Start time must be before end time");
        }

        if (StartTime.HasValue && StartTime.Value.Date < DateTime.UtcNow.Date)
        {
            if (errorBuilder.Length > 0) errorBuilder.Append(";\n");
            errorBuilder.Append("Start time cannot be in the past");
        }

        if (errorBuilder.Length > 0)
        {
            yield return Helper.CreateValidationResult(errorBuilder.ToString());
        }
    }
}