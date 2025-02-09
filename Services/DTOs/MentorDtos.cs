namespace Services.DTOs;

public class MentorDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public int Balance { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateMentorDto
{
    public string Code { get; set; } = null!;
    public int Balance { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int BaseSalaryPerHour { get; set; }
}

public class UpdateMentorDto
{
    public string? Code { get; set; }
    public int? Balance { get; set; }
    public string? BankName { get; set; }
    public string? BankCode { get; set; }
    public int? BaseSalaryPerHour { get; set; }
} 