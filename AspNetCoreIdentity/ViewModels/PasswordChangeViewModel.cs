using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreIdentity.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DisplayName("Eski şifreniz")]
        [Required(ErrorMessage = "Eski şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmalıdır.")]
        public string PasswordOld { get; set; }

        [DisplayName("Yeni şifreniz")]
        [Required(ErrorMessage = "Yeni şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmalıdır.")]
        public string PasswordNew { get; set; }

        [DisplayName("Yeni şifre tekrar")]
        [Required(ErrorMessage = "Yeni şifre onay gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmalıdır.")]
        [Compare("PasswordNew", ErrorMessage = "Yeni şifre ve onay şifreniz birbirinden farklıdır.")]
        public string PasswordConfirm { get; set; }
    }
}
