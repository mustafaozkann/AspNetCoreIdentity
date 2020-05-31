using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.TwoFactorServices
{
    public class EmailSender
    {
        private readonly TwoFactorOptions _twoFactorOptions;

        public EmailSender(IOptions<TwoFactorOptions> options)
        {
            _twoFactorOptions = options.Value;
        }

        public string Send(string email)
        {
            string code = "test123";
            Execute(email, code).Wait();
            return code;
        }

        private async Task Execute(string email, string code)
        {

            var client = new SendGridClient(_twoFactorOptions.SendGrid_ApiKey);
            var from = new EmailAddress("mstfa.ozkan6655@gmail.com", "Mustafa");
            var subject = "İki Adımlı Kimlik Doğrulama Kodunuz";
            var to = new EmailAddress(email);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<h2>Siteye giriş yapabilmek için doğrulama kodunuz aşağıdadır</h2><h3>Kodunuz : {code}</h3>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
