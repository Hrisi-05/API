using Microsoft.Extensions.Options;
using UserAPI.Interfaces;
using UserAPI.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MailKit;

namespace UserAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly SMTP _smtp;
        public EmailService(IOptions<SMTP> smtpOptions)
        {
            _smtp = smtpOptions.Value;
        }
        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_smtp.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.Auto);
            await client.AuthenticateAsync(_smtp.User, _smtp.Pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        
    }

    }

