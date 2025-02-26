using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using IModelValidator = Microsoft.EntityFrameworkCore.Infrastructure.IModelValidator;

namespace Services.Models.Request.Checkpoint;

public class CreateCheckpointRequest : IValidatableObject
{
    [Required] public string Name { get; set; } = null!;
    [Required] public Guid TermId { get; set; }
    [Required] public DateTime StartTime { get; set; }
    [Required] public DateTime EndTime { get; set; }

   public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errors = new List<string>();
        
        if (StartTime > EndTime)
        {
            errors.Add("Start time must be before end time");
        }

        if (StartTime.Date < DateTime.UtcNow.Date)
        {
            errors.Add("Start time cannot be in the past");
        }

        if (errors.Any())
        {
            yield return new ValidationResult(
                string.Join("; ", errors),
                new[] { nameof(StartTime), nameof(EndTime) }
            );
        }
    }
}