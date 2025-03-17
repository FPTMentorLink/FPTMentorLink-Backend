using System.ComponentModel.DataAnnotations;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Appointment;

public class UpdateAppointmentStatusRequest : ValidatableObject
{
    public AppointmentStatus Status { get; set; }
    public string? CancelReason { get; set; }
    public string? RejectReason { get; set; }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Status == AppointmentStatus.CancelRequested || Status == AppointmentStatus.Canceled && string.IsNullOrWhiteSpace(CancelReason))
            yield return new ValidationResult("Cancel reason is required");
        if (Status == AppointmentStatus.Rejected && string.IsNullOrWhiteSpace(RejectReason))
            yield return new ValidationResult("Reject reason is required");
    }
}