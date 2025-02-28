using Services.Utils.Email;

namespace Services.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, EmailContent content);
    Task SendEmailAsync(List<string> to, EmailContent content);
}