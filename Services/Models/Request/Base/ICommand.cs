using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.Base;

public interface ICommand : IValidatableObject
{
    
}

public abstract class Command : ICommand
{
    public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return [];
    }
}