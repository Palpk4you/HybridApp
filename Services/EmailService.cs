using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

public class EmailService : IEmailService, IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // Your custom interface method
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"],
            _configuration["EmailSettings:SenderEmail"]));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        email.Body = new TextPart("html") { Text = htmlBody };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            _configuration["EmailSettings:SmtpServer"],
            int.Parse(_configuration["EmailSettings:SmtpPort"]),
            MailKit.Security.SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _configuration["EmailSettings:SmtpUser"],
            _configuration["EmailSettings:SmtpPass"]);

        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    // Identity’s expected method
    async Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
    {
        await SendEmailAsync(email, subject, htmlMessage);
    }
}