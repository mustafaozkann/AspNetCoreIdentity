using AspNetCoreIdentity.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.ViewModels
{
    public class UserViewModel
    {

        [Required(ErrorMessage = "Kullanıcı adı boş geçilemez.")]
        [DisplayName("Kullanıcı Adı")]
        public string UserName { get; set; }

        [DisplayName("Tel No")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email alanı boş geçilemez.")]
        [DisplayName("Email adresi")]
        [EmailAddress(ErrorMessage = "Email adresiniz doğru formatta değil.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifre alanını doldurunuz")]
        [DisplayName("Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Lütfen şifre tekrar alanını doldurunuz")]
        [DisplayName("Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Şifre ve şifre tekrar alanları uyuşmuyor.")]
        public string RePassword { get; set; }

        [DisplayName("Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        public string Picture { get; set; }

        [DisplayName("Şehir")]
        public string City { get; set; }

        [DisplayName("Cinsiyet")]
        public Gender Gender { get; set; }
    }
}
