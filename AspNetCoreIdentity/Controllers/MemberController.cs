using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mapster;
using AspNetCoreIdentity.ViewModels;
using Mapster;
using AspNetCoreIdentity.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AspNetCoreIdentity.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {


        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {

        }

        [HttpGet]
        public IActionResult UserEdit()
        {
            AppUser user = CurrentUser;
            var userViewModel = user.Adapt<UserViewModel>();

            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
        {
            ModelState.Remove("Password");
            ModelState.Remove("RePassword");
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByNameAsync(User.Identity.Name);

                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        user.Picture = "/UserPicture/" + fileName;
                    }
                }
                user.UserName = userViewModel.UserName;
                user.Email = userViewModel.Email;
                user.PhoneNumber = userViewModel.PhoneNumber;
                user.City = userViewModel.City;
                user.BirthDay = userViewModel.BirthDay;
                user.Gender = (int)userViewModel.Gender;


                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    await signInManager.SignOutAsync();
                    await signInManager.SignInAsync(user, true);
                    ViewBag.success = "true";

                }
                else
                {
                    AddErrorsToModelState(result);

                }
            }
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));
            return View(userViewModel);
        }


        public IActionResult Index()
        {

            AppUser user = CurrentUser;
            var userViewModel = user.Adapt<UserViewModel>();
            return View(userViewModel);
        }

        [HttpGet]
        public IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = CurrentUser;

                bool exist = userManager.CheckPasswordAsync(user, passwordChangeViewModel.PasswordOld).Result;
                if (exist)
                {
                    IdentityResult result = userManager.ChangePasswordAsync(user, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew).Result;
                    if (result.Succeeded)
                    {
                        await userManager.UpdateSecurityStampAsync(user);
                        await signInManager.SignOutAsync();
                        await signInManager.PasswordSignInAsync(user, passwordChangeViewModel.PasswordNew, true, false);
                        ModelState.Clear();
                        ViewBag.success = "true";

                    }
                    else
                    {
                        AddErrorsToModelState(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Eski şifreniz yanlıştır");
                }

            }

            return View(passwordChangeViewModel);
        }

        public void LogOut()
        {
            signInManager.SignOutAsync();

        }




    }
}