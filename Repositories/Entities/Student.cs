using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Student : AuditableEntity
{
    [ForeignKey(nameof(Account))]
    public override Guid Id { get; set; }  // Override Id to be FK to Account

    [MaxLength(255)]
    public required string Code { get; set; }
    public int Balance { get; set; }

    public virtual Account Account { get; set; } = null!;
}