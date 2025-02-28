using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Services.Interfaces;
using Services.Utils.Email;

namespace Services.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(EmailSettings emailSettings)
    {
        _emailSettings = emailSettings;
    }

    public async Task SendEmailAsync(string to, EmailContent content)
    {
        await SendEmailAsync([to], content);
    }

    public async Task SendEmailAsync(List<string> to, EmailContent content)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
        email.To.AddRange(to.Select(x => new MailboxAddress("", x)));
        email.Subject = content.Subject;

        var builder = new BodyBuilder { HtmlBody = content.HtmlBody };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, 
            _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
        await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
} 