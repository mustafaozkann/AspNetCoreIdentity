using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.ViewModels
{
    public class ResetPasswordViewModel
    {
        [DisplayName("Email adresiniz")]
        [Required(ErrorMessage = "Email adresiniz gereklidir")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Yeni Şifreniz")]
        [Required(ErrorMessage = "Şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter olmalıdır.")]
        public string Password { get; set; }

        [DisplayName("Yeni Şifre Tekrar")]
        [Required(ErrorMessage = "Şifre tekrar gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakter olmalıdır.")]
        [Compare("Password", ErrorMessage = "Şifre alanları uyuşmuyor")]
        public string RePassword { get; set; }
    }
}
