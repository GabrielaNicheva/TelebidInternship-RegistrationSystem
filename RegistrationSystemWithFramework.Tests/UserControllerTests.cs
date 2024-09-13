using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RegistrationSystemWithFramework.Controllers;
using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Repository;
using RegistrationSystemWithFramework.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationSystemWithFramework.Tests
{
    public class UserControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private UserController _userController;
        private Mock<ISession> _sessionMock;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _sessionMock = new Mock<ISession>();

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(s => s.Session).Returns(_sessionMock.Object);

            _userController = new UserController(_userServiceMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        [TearDown]
        public void TearDown()
        {
            _userController.Dispose();
        }

        [Test]
        public void Registration_Get_ShouldReturnViewResult()
        {
            var result = _userController.Registration() as ViewResult;

            Assert.IsNotNull(result);
        }

        [Test]
        public void Login_Post_ShouldReturnView_WhenModelStateIsInvalid()
        {
            var model = new UserLoginViewModel();
            _userController.ModelState.AddModelError("Email", "Required");

            var result = _userController.Login(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
        }

        [Test]
        public void Login_Post_ShouldRedirectToIndex_WhenLoginSuccessful()
        {
            var model = new UserLoginViewModel
            {
                Email = "test@test.com",
                Password = "password123"
            };

            _userServiceMock.Setup(s => s.LoginCheck(It.IsAny<UserLoginViewModel>(), out It.Ref<string>.IsAny))
                .Returns(true);
            _userServiceMock.Setup(s => s.FindUser(It.IsAny<string>()))
                .Returns(new User { Email = model.Email, Name = "Test User" });

            var result = _userController.Login(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }


        [Test]
        public void VerifyCode_ShouldReturnRedirectToSuccessfulRegistration_WhenCodeIsValid()
        {
            _userServiceMock.Setup(s => s.CodeVerification(It.IsAny<string>(), out It.Ref<string>.IsAny))
                .Returns(true);

            var result = _userController.VerifyCode("123456") as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("SuccessfulRegistration", result.ActionName);
        }

    }
}
