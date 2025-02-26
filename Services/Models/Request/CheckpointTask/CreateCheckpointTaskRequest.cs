using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using System.Text;
using Services.Utils;

namespace Services.Models.Request.CheckpointTask;

public class CreateCheckpointTaskRequest
{
    [Required] public Guid CheckpointId { get; set; }

    [Required] public Guid ProjectId { get; set; }

    [Required][MaxLength(255)] public string Name { get; set; } = null!;

    [MaxLength(2000)] public string? Description { get; set; }

    [Required] public CheckpointTaskStatus Status { get; set; }

    public double? Score { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var errorBuilder = new StringBuilder();

        if (Score.HasValue && (Score < 0 || Score > 10))
        {
            errorBuilder.Append("Score must be between 0 and 10");
        }

        if (errorBuilder.Length > 0)
        {
            yield return Helper.CreateValidationResult(errorBuilder.ToString());
        }
    }
}