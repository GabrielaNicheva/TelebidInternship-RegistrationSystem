using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistrationSystemWithFramework.Captcha;
using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Service;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace RegistrationSystemWithFramework.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult GenerateCaptchaImage()
        {
            string captchaText = HttpContext.Session.GetString("CaptchaText");

            if (captchaText == null)
            {
                var captchaGenerator = new GenerateCaptcha();
                captchaText = captchaGenerator.GetCaptchaText();
                HttpContext.Session.SetString("CaptchaText", captchaText);
            }

            var captchaGeneratorImage = new GenerateCaptcha();
            using var captchaImage = captchaGeneratorImage.GenerateCaptchaImage(captchaText);

            using var memoryStream = new MemoryStream();
            captchaImage.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return File(memoryStream.ToArray(), "image/png");
        }

        public IActionResult Registration()
        {
            var captchaGenerator = new GenerateCaptcha();
            string captchaText = captchaGenerator.GetCaptchaText();

            HttpContext.Session.SetString("CaptchaText", captchaText);

            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserRegistrationViewModel model)
        {
            string captchaText = HttpContext.Session.GetString("CaptchaText");

            string verificationCode = GenerateVerificationCode();

            model.VerificationCode = verificationCode;
            model.IsVerified = false;

            if (model.Captcha is null || !model.Captcha.Equals(captchaText))
            {
                var captchaGenerator = new GenerateCaptcha();
                captchaText = captchaGenerator.GetCaptchaText();
                HttpContext.Session.SetString("CaptchaText", captchaText);
                ViewBag.ErrorMessage = "Invalid captcha.";
                ModelState.AddModelError(string.Empty, "Invalid captcha.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                var captchaGenerator = new GenerateCaptcha();
                captchaText = captchaGenerator.GetCaptchaText();
                HttpContext.Session.SetString("CaptchaText", captchaText);

                return View(model);
            }

            if (_userService.Add(model, out string errorMessage))
            {
                SendVerificationEmail(model.Email, verificationCode);

                return RedirectToAction("CheckYourEmail");
            }
            else
            {
                var captchaGenerator = new GenerateCaptcha();
                captchaText = captchaGenerator.GetCaptchaText();
                HttpContext.Session.SetString("CaptchaText", captchaText);

                ViewBag.ErrorMessage = errorMessage.Replace("\n", "<br>");

                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_userService.LoginCheck(model, out string errorMessage))
            {
                HttpContext.Session.SetString("UserEmail", model.Email);

                User user = _userService.FindUser(model.Email);
                HttpContext.Session.SetString("Name", user.Name);

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(model);
            }
        }

        public IActionResult Update()
        {
            string currentEmail = HttpContext.Session.GetString("UserEmail");
            if (String.IsNullOrEmpty(currentEmail))
            {
                return RedirectToAction("Error");
            }

            User user = _userService.FindUser(currentEmail);
            UserUpdateViewModel userLoginModel = new UserUpdateViewModel(user.Email, user.Name, user.Password);

            return View(userLoginModel);
        }

        [HttpPost]
        public IActionResult Update(UserUpdateViewModel userUpdateModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userUpdateModel);
            }

            User user = _userService.FindUser(userUpdateModel.Email);
            if (_userService.Update(userUpdateModel, user, out string errorMessage))
            {
                return RedirectToAction("UpdatedData");
            }

            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(userUpdateModel);
            }

        }

        public IActionResult UpdatedData()
        {
            string currentEmail = HttpContext.Session.GetString("UserEmail");
            User user = _userService.FindUser(currentEmail);
            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

        }

        [HttpPost]
        public IActionResult VerifyCode(string verificationCode)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_userService.CodeVerification(verificationCode, out string errorMessage))
            {
                return RedirectToAction("SuccessfulRegistration");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }
        }


        private string GenerateVerificationCode()
        {
            var random = new Random();
            var verificationCode = random.Next(100000, 999999).ToString();
            return verificationCode;
        }

        public void SendVerificationEmail(string toEmail, string verificationCode)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("regform83@gmail.com", "zzuz bayy xjbe mngv"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("regform83@gmail.com"),
                Subject = "Email Verification",
                Body = $"Your verification code is: {verificationCode}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }

        public IActionResult CheckYourEmail()
        {
            return View();
        }

        public IActionResult SuccessfulRegistration()
        {
            return View();
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (_userService.ForgotPassword(email, out string errorMessage))
            {
                var user = _userService.FindUser(email);
                var token = user.resetPassword;
                var resetUrl = Url.Action("ResetPassword", "User", new { token = token, email = user.Email }, "https");

                SendResetEmail(user.Email, resetUrl);

                return RedirectToAction("SendingEmail");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View();
            }
        }

        public void SendResetEmail(string email, string resetUrl)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("regform83@gmail.com", "zzuz bayy xjbe mngv"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("regform83@gmail.com"),
                Subject = "Reset password",
                Body = $"If you want to reset your password please follow this link {resetUrl}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }

        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordViewModel
            {
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (_userService.ResetPassword(resetPasswordViewModel, out string errorMessage))
            {
                return RedirectToAction("SuccessfulPasswordChange");
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
                return View(resetPasswordViewModel);
            }
        }

        public IActionResult SendingEmail()
        {
            return View();
        }

        public IActionResult SuccessfulPasswordChange()
        {
            return View();
        }
    }
}
