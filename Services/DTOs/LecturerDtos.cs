namespace Services.DTOs;

public class LecturerDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Faculty { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateLecturerDto
{
    public string Code { get; set; } = null!;
    public string? Description { get; set; }
    public string? Faculty { get; set; }
}

public class UpdateLecturerDto
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Faculty { get; set; }
} 