using AuthService.Application.IServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace AuthService.Application.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var smtpServer = _configuration["EmailSettings:SmtpHost"];
        //    var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
        //    var senderName = _configuration["EmailSettings:SenderName"];
        //    var senderEmail = _configuration["EmailSettings:SenderEmail"];
        //    var username = _configuration["EmailSettings:Username"];
        //    var password = _configuration["EmailSettings:Password"];

        //    var smtpClient = new SmtpClient(smtpServer)
        //    {
        //        Port = smtpPort,
        //        Credentials = new NetworkCredential(username, password),
        //        EnableSsl = true,
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress(senderEmail, senderName),
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true,
        //    };
        //    mailMessage.To.Add(toEmail);

        //    await smtpClient.SendMailAsync(mailMessage);
        //}

        public async Task SendEmailAsync(string toEmail, string subjects, string body)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("api-key", "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");

            var payload = new
            {
                sender = new { name = "TravelPlanner", email = "haonc.t2.1922@gmail.com" },
                to = new[] { new { email = toEmail } },
                subject = subjects,
                htmlContent = body
            };

            var response = await client.PostAsJsonAsync("https://api.brevo.com/v3/smtp/email", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {response.StatusCode} - {errorContent}");
            }
        }
    }
}
