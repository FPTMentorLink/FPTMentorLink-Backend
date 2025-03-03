namespace Services.Models.Response.Faculty;

public class FacultyResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}