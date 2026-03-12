using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace HybridApp.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For now, just log or debug output
            Console.WriteLine($"Sending email to {email}: {subject}");
            Console.WriteLine(htmlMessage);

            // In production, integrate with SMTP or SendGrid
            return Task.CompletedTask;
        }
    }
}