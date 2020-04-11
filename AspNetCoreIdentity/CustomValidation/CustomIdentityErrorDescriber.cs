using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.CustomValidation
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return (new IdentityError()
            {
                Code = "EmailExists",
                Description = $"{email} mail adresi daha önce kullanılmıştır."
            });
        }

        public override IdentityError InvalidUserName(string userName)
        {
            return (new IdentityError()
            {
                Code = "InvalidUserName",
                Description = $"{userName} geçersizdir."
            });
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return (new IdentityError()
            {
                Code = "DuplicateUserName",
                Description = $"{userName} daha önce kullanılmıştır."
            });
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return (new IdentityError()
            {
                Code = "PasswordTooShort",
                Description = $"Şifreniz en az {length} karakter uzunluğunda olmalıdır."
            });
        }

        public override IdentityError InvalidToken()
        {
            return (new IdentityError()
            {
                Code = "InvalidToken",
                Description = "Geçersiz token ! Bu token daha önce kullanılmıştır."
            });
        }

        
    }
}
