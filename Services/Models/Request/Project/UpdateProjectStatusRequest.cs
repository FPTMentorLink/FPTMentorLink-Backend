using Repositories.Entities;
using Services.Models.Request.Base;

namespace Services.Models.Request.Project;

public class UpdateProjectStatusRequest : ValidatableObject
{
    public ProjectStatus Status { get; set; }
}