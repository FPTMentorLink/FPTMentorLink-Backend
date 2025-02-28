using System.ComponentModel.DataAnnotations;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Faculty : AuditableEntity
{
    [MaxLength(255)] public string Code { get; set; } = null!;
    [MaxLength(255)] public string Name { get; set; } = null!;
}