using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AspNetCoreIdentity.Helper
{
    public static class PasswordReset
    {
        public static bool PasswordResetSendEmail(string link, string email)
        {
            //MailMessage mail = new MailMessage();

            //var smptClient = new SmtpClient("")

            bool IsMailSend = false;

            try
            {
                var credentials = new NetworkCredential("sofware.developer.test1@gmail.com", "Mustafaozkan#1");
                using (WebClient clientz = new WebClient())
                {
                    var mail = new MailMessage()
                    {
                        From = new MailAddress(credentials.UserName),
                        Subject = "Şifremi Unuttum",
                        Body = $"<h2>Şifrenizi yenilemek için linke tıklayarak yeni şifrenizi oluşturabilirsiniz.</h2><hr> <a href='{link}'> Şifre Yenileme Linki</a>"
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

        public static async Task PasswordResetSendEmailWithSendGrid(string link, string email)
        {
            var apiKey = "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";
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("mstfa.ozkan6655@gmail.com", "Mustafa");
            var subject = "Şifremi Unuttum";
            var to = new EmailAddress(email);
            //var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<h2> Şifrenizi yenilemek için linke tıklayarak yeni şifrenizi oluşturabilirsiniz.</ h2 ><hr><a href='{link}'> Şifre Yenileme Linki</ a > ";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
