using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;

namespace RegistrationSystemWithFramework.Service
{
    public interface IUserService
    {
        public bool Add(UserRegistrationViewModel userModel, out string errorMessage);
        public bool LoginCheck(UserLoginViewModel model, out string errorMessage);
        public User FindUser(string email);
        public bool Update(UserUpdateViewModel userUpdateViewModel, out string errorMessage);
    }
}
