using AspNetCoreIdentity.TwoFactorServices;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Helper
{
    public class EmailConfirmation
    {

        public static bool SendEmail(string link, string email)
        {
            bool IsMailSend = false;

            try
            {
                var credentials = new NetworkCredential("sofware.developer.test1@gmail.com", "Mustafaozkan#1");
                using (WebClient clientz = new WebClient())
                {
                    var mail = new MailMessage()
                    {
                        From = new MailAddress(credentials.UserName),
                        Subject = "Email Doğrulama",
                        Body = $"<h2>Email adresinizi doğrulamak için lütfen aşağıdaki linke tıklayınız.</h2><hr> <a href='{link}'> Email doğrulama linki</a>"
                    };
                    mail.IsBodyHtml = true;
                    mail.To.Add(email);

                    var client = new SmtpClient()
                    {
                        Port = 587,
                        Host = "smtp.gmail.com",
                        EnableSsl = true,
                        UseDefaultCredentials = true,
                        Credentials = credentials
                    };
                    client.EnableSsl = true;
                    client.Send(mail);
                    IsMailSend = true;
                }
            }
            catch (Exception ex)
            {
                IsMailSend = false;

            }

            return IsMailSend;
        }

        public static async Task SendEmailWithSendGrid(string link, string email)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("mstfa.ozkan6655@gmail.com", "Mustafa");
            var subject = "Email Doğrulama";
            var to = new EmailAddress(email);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<h2> Email adresinizi doğrulamak için lütfen aşağıdaki linke tıklayınız.</ h2 ><hr><a href='{link}'> Email doğrulama linki</ a > ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
