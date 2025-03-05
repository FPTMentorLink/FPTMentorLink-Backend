namespace Services.Models.Email;

public class EmailContent
{
    public string Subject { get; set; } = null!;
    public string HtmlBody { get; set; } = null!;
}