using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.AppointmentFeedback;

public class UpdateAppointmentFeedbackRequest : ValidatableObject
{
    [MaxLength(2000)] public string? Content { get; set; }
    [Range(1, 5)] public int? Rate { get; set; }
}