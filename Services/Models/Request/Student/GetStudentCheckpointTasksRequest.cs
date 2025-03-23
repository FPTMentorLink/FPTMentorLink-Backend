using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repositories.Entities;
using Services.Utils;

namespace Services.Models.Request.Student;

public class GetStudentCheckpointTasksRequest : PaginationParams
{
    [BindNever] public Guid? StudentId { get; set; }
    public CheckpointTaskStatus? Status { get; set; }
}