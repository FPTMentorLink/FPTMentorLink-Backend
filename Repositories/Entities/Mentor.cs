using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Mentor : AuditableEntity
{
    [ForeignKey(nameof(Account))] public override Guid Id { get; set; } // Override Id to be FK to Account

    [MaxLength(255)] public required string Code { get; set; }
    public int Balance { get; set; } //default value
    [MaxLength(255)] public string? BankName { get; set; }
    [MaxLength(255)] public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; } //default value

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<MentorAvailability> Availabilities { get; set; } = [];
    public virtual ICollection<Appointment> Appointments { get; set; } = [];
}