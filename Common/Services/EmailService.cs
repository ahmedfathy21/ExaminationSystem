using System.Net;
using System.Net.Mail;
using ExaminationSystem.Common.Models;
using Microsoft.Extensions.Options;

namespace ExaminationSystem.Common.Services;

public class MockEmailService : IEmailService
{
    public static List<EmailMessage> SentEmails = new();

    public Task SendEmailAsync(string toEmail, string subject, string body)
    {
        SentEmails.Add(new EmailMessage
        {
            ToEmail = toEmail,
            Subject = subject,
            Body = body,
            SentAt = DateTime.UtcNow
        });
        Console.WriteLine($"[MOCK EMAIL] To: {toEmail}, Subject: {subject}");
        return Task.CompletedTask;
    }
}

public class EmailMessage
{
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_settings.SenderEmail, _settings.SenderName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }
}