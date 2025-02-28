using Repositories.Entities.Base;

namespace Services.Models.Response.Base;

public abstract class AuditableResponse : IAuditable
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}