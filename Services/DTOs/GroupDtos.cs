namespace Services.DTOs;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateGroupDto
{
    public string Name { get; set; } = null!;
}

public class UpdateGroupDto
{
    public string? Name { get; set; }
} 