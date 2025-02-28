using System.ComponentModel.DataAnnotations;

namespace Services.Models.Request.AppointmentFeedback;

public class CreateAppointmentFeedbackRequest
{
    public Guid AppointmentId { get; set; }

    [Required] [MaxLength(2000)] public string Content { get; set; } = null!;

    [Required]
    [Range(1, 5)] // Assuming rating is from 1-5
    public int Rate { get; set; }
}