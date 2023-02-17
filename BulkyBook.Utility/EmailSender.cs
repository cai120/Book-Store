using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSend = new MimeMessage();
            emailSend.Subject = subject;
            emailSend.From.Add(MailboxAddress.Parse("cai.wing@guestia.co.uk"));
            emailSend.To.Add(MailboxAddress.Parse(email));
            emailSend.Body = new TextPart(MimeKit.Text.TextFormat.Html){ Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                client.Connect("testSMTP", 151, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("Username", "Password");
                client.SendAsync(emailSend);
            }

            return Task.CompletedTask;
        }
    }
}
