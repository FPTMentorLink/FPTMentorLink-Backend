using Repositories.Entities;

namespace Services.Models.Request.Project;

public class UpdateProjectStatusRequest
{
    public ProjectStatus? Status { get; set; }
}