using System.ComponentModel.DataAnnotations;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Group : AuditableEntity
{
    [MaxLength(255)] public required string Name { get; set; }
}