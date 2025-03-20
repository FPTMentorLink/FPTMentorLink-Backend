using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Student;

public class GetStudentAppointmentsRequest : PaginationQuery
{
    [BindNever] public Guid? StudentId { get; set; }
    public AppointmentStatus? Status { get; set; }
}