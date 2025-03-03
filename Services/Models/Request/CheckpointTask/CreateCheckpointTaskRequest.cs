using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using System.Text;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.CheckpointTask;

public class CreateCheckpointTaskRequest : ValidatableObject
{
    [Required] public Guid CheckpointId { get; set; }

    [Required] public Guid ProjectId { get; set; }

    [Required][MaxLength(255)] public string Name { get; set; } = null!;

    [MaxLength(2000)] public string? Description { get; set; }

    [Required] public CheckpointTaskStatus Status { get; set; }

    public double? Score { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Score is < 0 or > 10)
        {
            yield return Helper.CreateValidationResult("Score must be between 0 and 10");
        }
    }
}