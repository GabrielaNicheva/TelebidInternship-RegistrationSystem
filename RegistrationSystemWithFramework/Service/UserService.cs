using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Repository;
using System.Text.RegularExpressions;

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
            if (string.IsNullOrEmpty(userModel.Name))
            {
                errorMessage = "Name cannot be empty.";
                return false;
            }

            if (string.IsNullOrEmpty(userModel.Email))
            {
                errorMessage = "Email cannot be empty.";
                return false;
            }

            if (string.IsNullOrEmpty(userModel.Phone))
            {
                errorMessage = "Phone cannot be empty.";
                return false;
            }

            if (string.IsNullOrEmpty(userModel.Password))
            {
                errorMessage = "Password cannot be empty.";
                return false;
            }

            string emailRegex = @"^[\w\.-]+@[a-zA-Z\d\.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(userModel.Email, emailRegex))
            {
                errorMessage = "Invalid email";
                return false;
            }

            string phoneRegex = "^\\d+$";

            if (!Regex.IsMatch(userModel.Phone, phoneRegex))
            {
                errorMessage = "Invalid phone number";
                return false;
            }

            string passwordRegex = "^(?=.*\\d).{6,}$";

            if (!Regex.IsMatch(userModel.Password, passwordRegex))
            {
                errorMessage = "The password must contain at least 6 symbols and at least one digit";
                return false;
            }

            if (!userModel.Password.Equals(userModel.ConfirmedPassword))
            {
                errorMessage = "Passwords do not match.";
                return false;
            }

            var user = new User(userModel.Name, userModel.Email, userModel.Password, userModel.Gender, userModel.ISOCode, userModel.Phone, userModel.Address);
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

            if(!user.Password.Equals(model.Password))
            {
                errorMessage = "Wrong password";
                return false;
            }

            if(StaticCurrentEmail.CurrentEmail is not null)
            {
                errorMessage = "At first you must log out ";
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


    }
}
