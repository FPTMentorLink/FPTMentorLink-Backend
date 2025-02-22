using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Notification : AuditableEntity
{ 
    public string Content { get; set; } = null!;
    public string Title { get; set; } = null!;
    public AccountRole[] Roles { get; set; } = null!;
}