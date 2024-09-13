using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Repository;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegistrationSystemWithFramework.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository noteRepository)
        {
            _repository = noteRepository;
        }
        public bool Add(UserRegistrationViewModel userModel, out string errorMessage)
        {
            StringBuilder builder = new StringBuilder();
            bool error = false;

            if (string.IsNullOrEmpty(userModel.Name))
            {
                builder.AppendLine("Name cannot be empty.");
                error = true;
            }

            if (string.IsNullOrEmpty(userModel.Email))
            {
                builder.AppendLine("Email cannot be empty.");
                error = true;
            }
            else
            {
                string emailRegex = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(userModel.Email, emailRegex))
                {
                    builder.AppendLine("Invalid email");
                    error = true;
                }
            }

            if (string.IsNullOrEmpty(userModel.Phone))
            {
                builder.AppendLine("Phone cannot be empty.");
                error = true;
            }
            else
            {
                string phoneRegex = @"^\d+$";
                if (!Regex.IsMatch(userModel.Phone, phoneRegex))
                {
                    builder.AppendLine("Invalid phone number");
                    error = true;
                }
            }

            if (string.IsNullOrEmpty(userModel.Password))
            {
                builder.AppendLine("Password cannot be empty.");
                error = true;
            }
            else
            {
                string passwordRegex = @"^(?=.*\d).{6,}$";
                if (!Regex.IsMatch(userModel.Password, passwordRegex))
                {
                    builder.AppendLine("The password must contain at least 6 symbols and at least one digit");
                    error = true;
                }
            }

            if (!string.IsNullOrEmpty(userModel.Password) && !userModel.Password.Equals(userModel.ConfirmedPassword))
            {
                builder.AppendLine("Passwords do not match.");
                error = true;
            }

            User userAlreadyExists = FindUser(userModel.Email);
            if (userAlreadyExists != null)
            {
                builder.AppendLine("Account with this email already exists.");
                error = true;
            }

            if (error)
            {
                errorMessage = builder.ToString();
                return false;
            }
            var user = new User(userModel.Name, userModel.Email, userModel.Password, userModel.Gender, userModel.ISOCode, userModel.Phone, userModel.Address, userModel.IsVerified, userModel.VerificationCode);
            _repository.Add(user);

            errorMessage = null;
            return true;
        }


        public bool LoginCheck(UserLoginViewModel model, out string errorMessage)
        {
            User user = FindUser(model.Email);
           
            if (user is null)
            {
                errorMessage = "Invalid email";
                return false;
            }

            if (user.IsVerified == false || user.IsVerified is null)
            {
                errorMessage = "Your registration is not verified.";
                return false;
            }

            if (!user.Password.Equals(model.Password))
            {
                errorMessage = "Wrong password";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public User FindUser(string email)
        {
            return _repository.FindByEmail(email);
        }

        public bool Update(UserUpdateViewModel userUpdateViewModel, User user, out string errorMessage)
        {
            string passwordRegex = "^(?=.*\\d).{6,}$";

            if (!Regex.IsMatch(userUpdateViewModel.Password, passwordRegex))
            {
                errorMessage = "The password must contain at least 6 symbols and at least one digit";
                return false;
            }

			if (user.Name.Equals(userUpdateViewModel.Name) && user.Password.Equals(userUpdateViewModel.Password))
			{
				errorMessage = "Neither name nor password was changed";
				return false;
			}

			_repository.Update(userUpdateViewModel);
            errorMessage = null;
            return true;
        }

        public bool CodeVerification(string verificationCode, out string errorMessage)
        {
            bool isRegistered = _repository.CodeVerification(verificationCode);
            if (isRegistered)
            {
                errorMessage = null;
                return true;
            }
            errorMessage = "Invalid verification code.";
            return false;
        }

        public bool ForgotPassword(string email, out string errorMessage)
        {
            bool isPasswordForgotSucceeded = _repository.ForgotPassword(email);
            if(!isPasswordForgotSucceeded)
            {
                errorMessage = "This email is not registered";
                return false;
            }
            errorMessage = null;
            return true;
        }

        public bool ResetPassword(ResetPasswordViewModel resetPasswordViewModel, out string errorMessage)
        {
            string passwordRegex = "^(?=.*\\d).{6,}$";

            if (!Regex.IsMatch(resetPasswordViewModel.Password, passwordRegex))
            {
                errorMessage = "The password must contain at least 6 symbols and at least one digit";
                return false;
            }

            if (resetPasswordViewModel.Password != resetPasswordViewModel.ConfirmedPassword)
            {
                errorMessage = "Passwords do not match.";
                return false;
            }

            if(!_repository.ResetPassword(resetPasswordViewModel.Token, resetPasswordViewModel.Email, resetPasswordViewModel.Password))
            {
                errorMessage = "Error with the token or with the user";
                return false;
            }

            errorMessage = null;
            return true;

        }


    }
}
