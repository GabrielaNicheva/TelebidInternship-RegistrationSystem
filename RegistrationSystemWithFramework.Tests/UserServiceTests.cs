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
            var userModel = new UserRegistrationViewModel { Name = "", Email = "test@example.com", Phone = "123456", Password = "password1", ConfirmedPassword = "password1" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Name cannot be empty.\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenEmailIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "", Phone = "123456", Password = "password1", ConfirmedPassword = "password1" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Email cannot be empty.\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPhoneIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "", Password = "password1", ConfirmedPassword = "password1" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Phone cannot be empty.\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordIsEmpty_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "", ConfirmedPassword = "" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Password cannot be empty.\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenEmailIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "invalid-email", Phone = "123456", Password = "password1", ConfirmedPassword = "password1" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid email\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPhoneIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "invalid-phone", Password = "password1", ConfirmedPassword = "password1" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid phone number\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "pass", ConfirmedPassword = "pass" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordsDoNotMatch_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "password1", ConfirmedPassword = "password2" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Passwords do not match.\r\n", errorMessage);
        }

        [Test]
        public void Add_WhenPasswordIsInvalidAndDoNotMatch_ShouldReturnFalseAndErrorMessage()
        {
            var userModel = new UserRegistrationViewModel { Name = "Test", Email = "test@example.com", Phone = "123456", Password = "pass", ConfirmedPassword = "password2" };
            var result = _userService.Add(userModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit\r\nPasswords do not match.\r\n", errorMessage);
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
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, true, "111111");
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "wrongpassword" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Wrong password", errorMessage);
        }

        [Test]
        public void LoginCheck_WhenUsersRegistrationIsNotVerified_ShouldReturnFalseAndErrorMessage()
        {
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, false, "");
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

            var model = new UserLoginViewModel { Email = "test@example.com", Password = "wrongpassword" };
            var result = _userService.LoginCheck(model, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Your registration is not verified.", errorMessage);
        }

        [Test]
        public void LoginCheck_WhenAllValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, true, "111111");
            _repositoryMock.Setup(r => r.FindByEmail(It.IsAny<string>())).Returns(user);

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
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, true, "111111");

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit", errorMessage);
        }

        [Test]
        public void Update_WhenNoChanges_ShouldReturnFalseAndErrorMessage()
        {
            var userUpdateViewModel = new UserUpdateViewModel { Name = "Test", Password = "password1" };
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, true, "111111");

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Neither name nor password was changed", errorMessage);
        }

        [Test]
        public void Update_WhenAllValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var userUpdateViewModel = new UserUpdateViewModel { Name = "New Name", Password = "newpassword1" };
            var user = new User("Test", "test@example.com", "password1", null, null, "123456", null, true, "111111");

            _repositoryMock.Setup(r => r.Update(userUpdateViewModel)).Verifiable();

            var result = _userService.Update(userUpdateViewModel, user, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
            _repositoryMock.Verify(r => r.Update(userUpdateViewModel), Times.Once);
        }

        #endregion

        #region CodeVerification
        [Test]
        public void CodeVerification_WhenCodeIsValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var verificationCode = "validCode";
            _repositoryMock.Setup(r => r.CodeVerification(verificationCode)).Returns(true);

            var result = _userService.CodeVerification(verificationCode, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        [Test]
        public void CodeVerification_WhenCodeIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var verificationCode = "invalidCode";
            _repositoryMock.Setup(r => r.CodeVerification(verificationCode)).Returns(false);

            var result = _userService.CodeVerification(verificationCode, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Invalid verification code.", errorMessage);
        }
        #endregion

        #region ForgotPassword
        [Test]
        public void ForgotPassword_WhenEmailIsRegistered_ShouldReturnTrueAndNoErrorMessage()
        {
            var email = "test@example.com";
            _repositoryMock.Setup(r => r.ForgotPassword(email)).Returns(true);

            var result = _userService.ForgotPassword(email, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        [Test]
        public void ForgotPassword_WhenEmailIsNotRegistered_ShouldReturnFalseAndErrorMessage()
        {
            var email = "notregistered@example.com";
            _repositoryMock.Setup(r => r.ForgotPassword(email)).Returns(false);

            var result = _userService.ForgotPassword(email, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("This email is not registered", errorMessage);
        }

        #endregion

        #region ResetPassword
        [Test]
        public void ResetPassword_WhenPasswordIsValid_ShouldReturnTrueAndNoErrorMessage()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Password = "password1",
                ConfirmedPassword = "password1",
                Token = "validToken",
                Email = "test@example.com"
            };
            _repositoryMock.Setup(r => r.ResetPassword(resetPasswordViewModel.Token, resetPasswordViewModel.Email, resetPasswordViewModel.Password)).Returns(true);

            var result = _userService.ResetPassword(resetPasswordViewModel, out var errorMessage);

            Assert.IsTrue(result);
            Assert.IsNull(errorMessage);
        }

        [Test]
        public void ResetPassword_WhenPasswordIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Password = "pass",
                ConfirmedPassword = "pass",
                Token = "validToken",
                Email = "test@example.com"
            };

            var result = _userService.ResetPassword(resetPasswordViewModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("The password must contain at least 6 symbols and at least one digit", errorMessage);
        }

        [Test]
        public void ResetPassword_WhenPasswordsDoNotMatch_ShouldReturnFalseAndErrorMessage()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Password = "password1",
                ConfirmedPassword = "password2",
                Token = "validToken",
                Email = "test@example.com"
            };

            var result = _userService.ResetPassword(resetPasswordViewModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Passwords do not match.", errorMessage);
        }

        [Test]
        public void ResetPassword_WhenTokenIsInvalid_ShouldReturnFalseAndErrorMessage()
        {
            var resetPasswordViewModel = new ResetPasswordViewModel
            {
                Password = "password1",
                ConfirmedPassword = "password1",
                Token = "invalidToken",
                Email = "test@example.com"
            };
            _repositoryMock.Setup(r => r.ResetPassword(resetPasswordViewModel.Token, resetPasswordViewModel.Email, resetPasswordViewModel.Password)).Returns(false);

            var result = _userService.ResetPassword(resetPasswordViewModel, out var errorMessage);

            Assert.IsFalse(result);
            Assert.AreEqual("Error with the token or with the user", errorMessage);
        }
        #endregion
    }
}
