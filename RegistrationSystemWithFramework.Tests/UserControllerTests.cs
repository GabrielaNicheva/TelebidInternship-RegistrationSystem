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

        //[Test]
        //public void Registration_Post_ShouldAddModelError_WhenCaptchaIsInvalid()
        //{
        //    // Arrange
        //    var model = new UserRegistrationViewModel { Captcha = "123456" };

        //    // Act
        //    var result = _userController.Registration(model) as ViewResult;

        //    // Assert
        //    Assert.IsFalse(_userController.ModelState.IsValid);
        //    Assert.AreEqual("Invalid captcha.", _userController.ModelState[string.Empty].Errors[0].ErrorMessage);
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(model, result.Model);
        //}

        //[Test]
        //public void Registration_Post_ShouldRedirectToCheckYourEmail_WhenModelIsValid()
        //{
        //    // Arrange
        //    var model = new UserRegistrationViewModel
        //    {
        //        Captcha = "1234",
        //        Email = "test@test.com"
        //    };

        //    _sessionMock.Setup(s => s.GetString("CaptchaText")).Returns("1234");
        //    _userServiceMock.Setup(s => s.Add(It.IsAny<UserRegistrationViewModel>(), out It.Ref<string>.IsAny))
        //        .Returns(true);

        //    // Act
        //    var result = _userController.Registration(model) as RedirectToActionResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("CheckYourEmail", result.ActionName);
        //}

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

        //[Test]
        //public void Login_Post_ShouldReturnView_WhenLoginFails()
        //{
        //    // Arrange
        //    var model = new UserLoginViewModel
        //    {
        //        Email = "test@test.com",
        //        Password = "password123"
        //    };

        //    _userServiceMock.Setup(s => s.LoginCheck(It.IsAny<UserLoginViewModel>(), out It.Ref<string>.IsAny))
        //        .Returns(false);
        //    _userServiceMock.Setup(s => s.FindUser(It.IsAny<string>()))
        //        .Returns((User)null);

        //    // Act
        //    var result = _userController.Login(model) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(model, result.Model);
        //}

        //[Test]
        //public void Update_ShouldReturnRedirectToError_WhenSessionEmailIsNull()
        //{
        //    // Arrange
        //    _sessionMock.Setup(s => s.GetString("UserEmail")).Returns((string)null);

        //    // Act
        //    var result = _userController.Update() as RedirectToActionResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Error", result.ActionName);
        //}

        //[Test]
        //public void Update_Post_ShouldReturnViewWithError_WhenUpdateFails()
        //{
        //    // Arrange
        //    var model = new UserUpdateViewModel
        //    {
        //        Email = "test@test.com",
        //        Name = "New Name",
        //        Password = "newpassword"
        //    };
        //    var user = new User
        //    {
        //        Email = "test@test.com",
        //        Name = "Test User",
        //        Password = "password123"
        //    };

        //    _userServiceMock.Setup(s => s.FindUser(It.IsAny<string>())).Returns(user);
        //    _userServiceMock.Setup(s => s.Update(It.IsAny<UserUpdateViewModel>(), user, out It.Ref<string>.IsAny))
        //        .Returns(false);

        //    // Act
        //    var result = _userController.Update(model) as ViewResult;

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(model, result.Model);
        //}

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
