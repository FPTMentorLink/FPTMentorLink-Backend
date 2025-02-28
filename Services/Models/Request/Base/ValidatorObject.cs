using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.Base;


public abstract class ValidatorObject : IValidatableObject
{
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}