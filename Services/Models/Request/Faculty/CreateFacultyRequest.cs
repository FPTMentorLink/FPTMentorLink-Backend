using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.Faculty;

public class CreateFacultyRequest : ValidatableObject
{
    [MaxLength(255)] public string Code { get; set; } = null!;
    [MaxLength(255)] public string Name { get; set; } = null!;
}