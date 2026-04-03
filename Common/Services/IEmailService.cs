using System.Threading.Tasks;

namespace ExaminationSystem.Common.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}
