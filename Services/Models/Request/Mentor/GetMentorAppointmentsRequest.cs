using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Utils;

namespace Services.Models.Request.Mentor;

public class GetMentorAppointmentsRequest : PaginationParams
{
    [BindNever] public Guid? MentorId { get; set; }
    public AppointmentStatus? Status { get; set; }
}