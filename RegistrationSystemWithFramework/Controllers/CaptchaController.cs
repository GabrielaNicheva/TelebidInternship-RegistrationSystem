using Microsoft.AspNetCore.Mvc;
using RegistrationSystemWithFramework.Captcha;

namespace RegistrationSystemWithFramework.Controllers
{
    public class CaptchaController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CaptchaController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Generate()
        {
            var captchaGenerator = new GenerateCaptcha();
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "captchas", "captcha.png");
            captchaGenerator.CreateCaptcha(filePath);
            byte[] imageData = System.IO.File.ReadAllBytes(filePath);
            return View("Registration", new {ImagePath = "/captchas/captcha.png"});
        }
    }
}
