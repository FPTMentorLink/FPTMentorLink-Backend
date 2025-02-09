namespace Services.DTOs;

public class TaskLogDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public TaskStatus Status { get; set; }
    public Guid SetBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTaskLogDto
{
    public Guid TaskId { get; set; }
    public TaskStatus Status { get; set; }
    public Guid SetBy { get; set; }
}

public class UpdateTaskLogDto
{
    public TaskStatus? Status { get; set; }
} 