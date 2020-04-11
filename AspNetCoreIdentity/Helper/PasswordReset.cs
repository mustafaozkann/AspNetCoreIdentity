using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

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
                var credentials = new NetworkCredential("mstfa.ozkan6655@gmail.com", "Mustafa.17");
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
    }
}
