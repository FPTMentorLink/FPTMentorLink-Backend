using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Student;

public class GetMyAppointmentsRequest : PaginationQuery
{
    [BindNever] public Guid? AccountId { get; set; }
    public AppointmentStatus? Status { get; set; }
}