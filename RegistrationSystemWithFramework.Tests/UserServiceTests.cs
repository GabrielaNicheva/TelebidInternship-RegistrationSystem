using Moq;
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
    public class UserServiceTests
    {
        private Mock<IUserRepository> _repositoryMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_repositoryMock.Object);
        }

        #region Registration
        [Test]
        public void Add_WhenNameIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Name cannot be empty.", errorMessage);
        }

        [Test]
        public void Add_WhenEmailIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Email cannot be empty.", errorMessage);
        }

        [Test]
        public void Add_WhenPhoneIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Phone cannot be empty.", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Password cannot be empty.", errorMessage);
        }

        [Test]
        public void Add_WhenEmailIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "invalid-email", Phone = "123456", Password = "password" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid email", errorMessage);
        }

        [Test]
        public void Add_WhenPhoneIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "invalid-phone", Password = "password" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid phone number", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "pass" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordsDoNotMatch_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel
            {
                Name = "Test",
                Email = "test@example.com",
                Phone = "123456",
                Password = "password1",
                ConfirmedPassword = "password2"
            };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Passwords do not match.", errorMessage);
        }

        [Test]
        public void Add_WhenAllValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var userModel = new UserRegistrationViewModel
            {
                Name = "Test",
                Email = "test@example.com",
                Phone = "123456",
                Password = "password1",
                ConfirmedPassword = "password1"
            };

            _repositoryMock.Setup(r => r.Add(It.IsAny<User>())).Verifiable();

            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
            _repositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
        }
        #endregion

        #region Login

        [Test]
        public void LoginCheck_WhenUserNotFound_ShouldReturnFalseAndErrorMessage()
        {
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns((User)null);

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "password1" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid email", errorMessage);
        }

        [Test]
        public void LoginCheck_WhenPasswordIsWrong_ShouldReturnFalseAndErrorMessage()
        {
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "wrongpassword" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Wrong password", errorMessage);
        }

        [Test]
        public void LoginCheck_WhenAlreadyLoggedIn_ShouldReturnFalseAndErrorMessage()
        {
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

            StaticCurrentEmail.CurrentEmail = "test@example.com";

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "password1" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("At first you must log out ", errorMessage);
        }

        [Test]
        public void LoginCheck_WhenAllValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

            StaticCurrentEmail.CurrentEmail = null;

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "password1" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        #endregion

        #region Update

        [Test]
        public void Update_WhenPasswordIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userUpdateViewModel = new UserUpdateViewModel { Password = "short" };
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit", errorMessage);
        }

        [Test]
        public void Update_WhenNoChanges_ShouldReturnFalseAndErrorMessage()
        {
            var userUpdateViewModel = new UserUpdateViewModel { Name = "Test", Password = "password1" };
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Neither name nor password was changed", errorMessage);
        }

        [Test]
        public void Update_WhenAllValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var userUpdateViewModel = new UserUpdateViewModel { Name = "New Name", Password = "newpassword1" };
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null);

            _repositoryMock.Setup(r => r.Update(userUpdateViewModel)).Verifiable();

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
            _repositoryMock.Verify(r => r.Update(userUpdateViewModel), Times.Once);
        }

        #endregion
    }
}
