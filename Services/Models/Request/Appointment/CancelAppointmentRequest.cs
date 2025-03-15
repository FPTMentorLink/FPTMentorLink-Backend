using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Services.Models.Request.Base;
using Services.Utils;

namespace Services.Models.Request.Appointment;

public class CancelAppointmentRequest : ValidatableObject
{
    [JsonIgnore] public Guid UserId { get; set; }
    [JsonIgnore] public Guid AppointmentId { get; set; }
    public string CancelReason { get; set; } = null!;
    public bool IsMentorCancel { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(CancelReason))
            yield return Helper.CreateValidationResult("Cancel reason is required");
    }
}