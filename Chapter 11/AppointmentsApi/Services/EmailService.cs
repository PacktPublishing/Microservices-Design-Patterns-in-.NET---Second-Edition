using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
}

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("HealthCare System", "no-reply@healthcare.com"));
        message.To.Add(new MailboxAddress("Patient",to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.mailserver.com", 587, false);
        await client.AuthenticateAsync("smtp_username", "smtp_password");
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
