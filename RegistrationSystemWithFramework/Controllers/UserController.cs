using Grpc.Core;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RegistrationSystemWithFramework.Captcha;
using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Service;
using System.Drawing.Imaging;

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

			if (model.Captcha is null || !model.Captcha.Equals(captchaText))
			{
				ModelState.AddModelError(string.Empty, "Invalid CAPTCHA.");
				return View();
			}

			if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_userService.Add(model, out string errorMessage))
            {
                return RedirectToAction("Login");
            }

            else
            {
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
            UserUpdateViewModel userLoginModel = new UserUpdateViewModel(user.Email,user.Name, user.Password);

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
            if(_userService.Update(userUpdateModel, user, out string errorMessage))
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
            UserUpdateViewModel userLoginModel = new UserUpdateViewModel(user.Email, user.Name, user.Password);
            return View(userLoginModel);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");

        }
    }
}
