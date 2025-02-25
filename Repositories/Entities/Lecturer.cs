using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Lecturer : AuditableEntity
{
    [ForeignKey(nameof(Account))] public override Guid Id { get; set; } // Override Id to be FK to Account

    [MaxLength(255)] public required string Code { get; set; }
    [MaxLength(2000)] public string? Description { get; set; }
    [ForeignKey(nameof(Faculty))] public Guid FacultyId { get; set; }

    public virtual Account Account { get; set; } = null!;
    public virtual Faculty Faculty { get; set; } = null!;
}