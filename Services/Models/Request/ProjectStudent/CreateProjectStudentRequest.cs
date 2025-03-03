using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.ProjectStudent;

public class CreateProjectStudentRequest : ValidatableObject
{
    [Required] public Guid StudentId { get; set; }
    [Required] public Guid ProjectId { get; set; }
    [Required] public bool IsLeader { get; set; }
}