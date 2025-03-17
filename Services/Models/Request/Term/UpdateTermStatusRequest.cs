using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Term;

public class UpdateTermStatusRequest : ValidatableObject
{
    public int Status { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!Enum.IsDefined(typeof(TermStatus), Status))
        {
            yield return Helper.CreateValidationResult("Invalid status value (Range: 1-3)");
        }
    }
}