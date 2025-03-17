using Repositories.Entities;
using Services.Models.Response.Base;

namespace Services.Models.Response.Term;

public class TermResponse : AuditableResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TermStatus Status { get; set; }
    public string StatusName => Status.ToString();
}