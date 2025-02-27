using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.Project;

public class UpdateProjectRequest : ValidatorObject
{
    [MaxLength(255)] public string? Name { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    public Guid? FacultyId { get; set; }
}