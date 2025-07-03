using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using HospitalManagementSystem_HMS_.BL.AuthRepo.IServices;
using HospitalManagementSystem_HMS_.Dtos.AuthDtos;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HospitalManagementSystem_HMS_.BL.AuthRepo.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
                {
                    throw new ArgumentException("Email parameters cannot be null or empty.");
                }
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                email.To.Add(MailboxAddress.Parse(toEmail));
                email.Subject = subject;

                email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = body
                };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch
            {
                return false;
            }
        }
        //public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(toEmail))
        //        {
        //            throw new ArgumentException("Recipient email address cannot be null or empty.", nameof(toEmail));
        //        }
        //        var message = new MailMessage
        //        {
        //            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };
        //        message.To.Add(toEmail);

        //        using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
        //        {
        //            Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password),
        //            EnableSsl = true
        //        };

        //        await client.SendMailAsync(message);
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
    }
}
