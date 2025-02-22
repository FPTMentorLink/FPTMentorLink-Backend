using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class AppointmentFeedback : AuditableEntity
{
    [ForeignKey(nameof(Appointment))] public Guid AppointmentId { get; set; }
    [ForeignKey(nameof(Student))] public Guid StudentId { get; set; }
    [MaxLength(2000)] public string Content { get; set; } = null!;
    public int Rate { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}