using Services.Models.Response.Base;

namespace Services.Models.Response.AppointmentFeedback;

public class AppointmentFeedbackResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid StudentId { get; set; }
    public string Content { get; set; } = null!;
    public int Rate { get; set; }
}