public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendEmailAsync(List<string> to, string subject, string htmlBody);
} 