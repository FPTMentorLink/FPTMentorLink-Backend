using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.Base;

public abstract class ValidatableObject : IValidatableObject
{
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}