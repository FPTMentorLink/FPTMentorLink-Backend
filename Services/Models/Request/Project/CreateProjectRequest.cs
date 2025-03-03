using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.Project;

public class CreateProjectRequest : ValidatableObject
{
    [Required] [MaxLength(255)] public string Name { get; set; } = null!;
    [MaxLength(2000)] public string? Description { get; set; }

    [Required] public Guid FacultyId { get; set; }

    //TODO: Should take LeaderId from JWT claim for security
    [Required] public Guid LeaderId { get; set; }
    public Guid? LecturerId { get; set; }
}