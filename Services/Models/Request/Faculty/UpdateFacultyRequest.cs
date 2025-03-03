using System.ComponentModel.DataAnnotations;
using Services.Models.Request.Base;

namespace Services.Models.Request.Faculty;

public class UpdateFacultyRequest : ValidatableObject
{
    [MaxLength(255)] public string? Code { get; set; }
    [MaxLength(255)] public string? Name { get; set; }
}