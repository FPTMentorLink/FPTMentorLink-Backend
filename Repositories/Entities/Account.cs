using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration.Attributes;
using Repositories.Entities.Base;

namespace Repositories.Entities;

public class Account : AuditableEntity
{
    [MaxLength(255)] public required string Email { get; set; }
    [MaxLength(255)] public required string Username { get; set; }
    [MaxLength(255)] public required string PasswordHash { get; set; }
    [MaxLength(255)] public required string FirstName { get; set; }
    [MaxLength(255)] public required string LastName { get; set; }
    [MaxLength(255)] public string? ImageUrl { get; set; }

    public AccountRole Role { get; set; }
    public bool IsSuspended { get; set; }

    // Navigation properties for 1:1 relationships
    public virtual Student? Student { get; set; }
    public virtual Mentor? Mentor { get; set; }
    public virtual Lecturer? Lecturer { get; set; }
}

public abstract class CsvAccount
{
    [Name("Email")]
    public string Email { get; set; } = null!;

    [Name("Username")] public string Username { get; set; } = null!;
    [Name("Password")] public string Password { get; set; } = null!;
    [Name("FirstName")] public string FirstName { get; set; } = null!;
    [Name("LastName")] public string LastName { get; set; } = null!;
}

public class CsvStudent : CsvAccount
{
    [Name("StudentCode")] public string Code { get; set; } = null!;
    [Name("FacultyCode")] public string FacultyCode { get; set; } = null!;
}

public class CsvMentor : CsvAccount
{
    [Name("Code")] public string Code { get; set; } = null!;
}

public class CsvLecturer : CsvAccount
{
    [Name("LecturerCode")] public string Code { get; set; } = null!;
    [Name("FacultyCode")] public string FacultyCode { get; set; } = null!;

}

public enum AccountRole
{
    Admin = 1,
    Student = 2,
    Mentor = 3,
    Lecturer = 4
}