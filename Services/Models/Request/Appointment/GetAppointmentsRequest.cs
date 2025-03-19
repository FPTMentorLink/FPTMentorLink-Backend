using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Appointment;

public class GetAppointmentsRequest : PaginationQuery
{
    public Guid? StudentId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? MentorId { get; set; }
    public AppointmentStatus? Status { get; set; }
}