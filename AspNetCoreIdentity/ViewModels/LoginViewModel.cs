using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Email adresiniz")]
        [Required(ErrorMessage = "Email adresiniz gereklidir")]
        [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değil.")]
        public string Email { get; set; }

        [DisplayName("Şifreniz")]
        [Required(ErrorMessage = "Şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter olmalıdır.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
