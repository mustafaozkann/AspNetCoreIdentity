using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreIdentity.Helper;

using AspNetCoreIdentity.ViewModels;
using DataAccess.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhoneNumbers;

namespace AspNetCoreIdentity.Controllers
{

    public class HomeController : BaseController
    {

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {

        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var IsLockOut = await userManager.IsLockedOutAsync(user);
                    if (IsLockOut)
                    {
                        ModelState.AddModelError("", "Hesabınız bir süreliğine askıya alınmıştır. Lütfen daha sonra tekrar deneyiniz.");
                        return View(model);
                    }

                    var isPasswordCorrect = await userManager.CheckPasswordAsync(user, model.Password);
                    if (isPasswordCorrect)
                    {
                        if (!await userManager.IsEmailConfirmedAsync(user))
                        {
                            ModelState.AddModelError("", "Hesabınız doğrulanmamıştır. Lütfen gelen emaile tıklayarak hesabınızı doğrulayınız.");
                            return View(model);
                        }

                        await signInManager.SignOutAsync();
                        var result = await signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);

                        if (result.Succeeded)
                        {
                            //await userManager.ResetAccessFailedCountAsync(user);
                            return RedirectToAction("Index", "Member");
                        }
                        else
                        {

                            //await userManager.AccessFailedAsync(user);
                            var failedCount = await userManager.GetAccessFailedCountAsync(user);
                            if (failedCount == 3)
                            {
                                await userManager.SetLockoutEndDateAsync(user, new DateTimeOffset(DateTime.Now.AddSeconds(30)));
                                ModelState.AddModelError("", "Hesabınız 3 başarısız girişten dolayı 20 dakika süreyle kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz");
                            }
                            else
                            {
                                ModelState.AddModelError("", "E-posta adresiniz ya da şifreniz yanlış.");
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "E-posta adresiniz ya da şifreniz yanlış.");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "E-posta adresiniz ya da şifreniz yanlış.");
                }
            }
            return View(model);
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
                string telephoneNumber = userViewModel.PhoneNumber;
                string countryCode = "TR";
                PhoneNumbers.PhoneNumber phoneNumber = phoneUtil.Parse(telephoneNumber, countryCode);

                bool isValidRegion = phoneUtil.IsValidNumberForRegion(phoneNumber, countryCode);

                if (!isValidRegion)
                {
                    ModelState.AddModelError("", "Geçerli telefon numarası giriniz");
                    return View(userViewModel);
                }

                AppUser user = new AppUser()
                {
                    UserName = userViewModel.UserName,
                    Email = userViewModel.Email,
                    PhoneNumber = userViewModel.PhoneNumber
                };

                IdentityResult result = await userManager.CreateAsync(user, userViewModel.Password);
                if (result.Succeeded)
                {
                    string emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    string confirmationLink = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = user.Id,
                        token = emailConfirmationToken

                    }, protocol: HttpContext.Request.Scheme);

                    EmailConfirmation.SendEmail(confirmationLink, user.Email);

                    return RedirectToAction("Login");
                }
                else
                {
                    AddErrorsToModelState(result);
                }
            }
            return View(userViewModel);
        }


        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([Bind(nameof(ResetPasswordViewModel.Email))]ResetPasswordViewModel model)
        {

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                string passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token = passwordResetToken

                }, HttpContext.Request.Scheme);
                // url/Home/ResetPasswordConfirm?userId=1234&token=asd123

                var IsMailSend = PasswordReset.PasswordResetSendEmail(passwordResetLink, user.Email);
                ViewBag.Status = "success";
            }
            else
            {
                ModelState.AddModelError("", "Sistemde böyle kayıtlı bir mail adresi bulunamadı.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind(nameof(ResetPasswordViewModel.Password), nameof(ResetPasswordViewModel.RePassword))] ResetPasswordViewModel resetPasswordViewModel)
        {
            string token = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();

            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await userManager.ResetPasswordAsync(user, token, resetPasswordViewModel.Password);
                if (result.Succeeded)
                {
                    await userManager.UpdateSecurityStampAsync(user);
                    TempData["passwordResetInfo"] = "Şifreniz başarıyla yenilenmiştir. Yeni şifreniz ile giriş yapabilirsiniz";
                    ViewBag.status = "success";
                }
                else
                {
                    TempData["userId"] = userId;
                    TempData["token"] = token;
                    AddErrorsToModelState(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Hata meydana gelmiştir. Lütfen daha sonra tekrar deneyiniz.");
                TempData["userId"] = userId;
                TempData["token"] = token;
            }

            return View(resetPasswordViewModel);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                IdentityResult result = await userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    ViewBag.status = "Email adresiniz onaylanmıştır. Login ekranından giriş yapabilirsiniz.";
                }
                else
                {
                    ViewBag.status = "Bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz";
                }
            }
            return View();
        }


        public IActionResult FacebookLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);

            return new ChallengeResult("Facebook", properties);
        }

        public IActionResult GoogleLogin(string returnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Home", new { returnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return new ChallengeResult("Google", properties);
        }

        public async Task<IActionResult> ExternalResponse(string returnUrl = "/")
        {
            ExternalLoginInfo info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);

                if (result.Succeeded)
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    AppUser user = new AppUser();
                    user.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string ExternalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    if (info.Principal.HasClaim(x => x.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(' ', '-').ToLower() + ExternalUserId.Substring(0, 5).ToString();
                        user.UserName = userName;
                    }
                    else
                    {
                        user.UserName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    }

                    IdentityResult createResult = await userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        IdentityResult loginResult = await userManager.AddLoginAsync(user, info);

                        if (loginResult.Succeeded)
                        {
                            //await signInManager.SignInAsync(user, true);
                            //alternative login
                            await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            AddErrorsToModelState(loginResult);
                        }
                    }
                    else
                    {
                        AddErrorsToModelState(createResult);
                    }
                }
            }
            List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(y => y.ErrorMessage).ToList();
            return View("Error", errors);
        }

        public ActionResult Error()
        {
            return View();
        }



    }
}