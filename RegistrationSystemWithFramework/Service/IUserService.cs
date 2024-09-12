using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;

namespace RegistrationSystemWithFramework.Service
{
    public interface IUserService
    {
        public bool Add(UserRegistrationViewModel userModel, out string errorMessage);
        public bool LoginCheck(UserLoginViewModel model, out string errorMessage);
        public User FindUser(string email);
        public bool Update(UserUpdateViewModel userUpdateViewModel,User user, out string errorMessage);
        public bool CodeVerification(string verificationCode, out string errorMessage);
        public bool ForgotPassword(string email, out string errorMessage);
        public bool ResetPassword(ResetPasswordViewModel resetPasswordViewModel, out string errorMessage);
    }
}
