using Microsoft.AspNetCore.Mvc;
using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Service;

namespace RegistrationSystemWithFramework.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserService _userService;
        string? emailLogged = null;

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

        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(UserRegistrationViewModel model)
        {

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
                StaticCurrentEmail.CurrentEmail = model.Email;
                string currentEmail = StaticCurrentEmail.CurrentEmail;
                User user = _userService.FindUser(currentEmail);
                StaticCurrentEmail.CurrentName = user.Name;
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
            string currentEmail = StaticCurrentEmail.CurrentEmail;
            if (String.IsNullOrEmpty(currentEmail))
            {
                return RedirectToAction("Error");
            }

            User user = _userService.FindUser(currentEmail);
            UserUpdateViewModel userLoginModel = new UserUpdateViewModel(user.Email,user.Name, user.Password);

            var message = TempData["Message"] as string;
            ViewBag.Message = message;

            return View(userLoginModel);
        }

        [HttpPost]
        public IActionResult Update(UserUpdateViewModel userUpdateModel)
        {
            User user = _userService.FindUser(userUpdateModel.Email);
            if(user.Name.Equals(userUpdateModel.Name) && user.Password.Equals(userUpdateModel.Password))
            {
                TempData["Message"] = "Neither name nor password was changed";
                return RedirectToAction("Update");
            }
            _userService.Update(userUpdateModel, out string errorMessage);
           return RedirectToAction("UpdatedData");
        }

        public IActionResult UpdatedData()
        {
            string currentEmail = StaticCurrentEmail.CurrentEmail;
            User user = _userService.FindUser(currentEmail);
            UserUpdateViewModel userLoginModel = new UserUpdateViewModel(user.Email, user.Name, user.Password);
            return View(userLoginModel);
        }

        public IActionResult Logout()
        {
            StaticCurrentEmail.CurrentEmail = null;
            StaticCurrentEmail.CurrentName = null;
            return RedirectToAction("Index", "Home");

        }
    }
}
