using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.Helper
{
    public static class EmailConfirmation
    {
        public static bool SendEmail(string link, string email)
        {
            bool IsMailSend = false;

            try
            {
                var credentials = new NetworkCredential("emailName@gmail.com", "***");
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
    }
}
