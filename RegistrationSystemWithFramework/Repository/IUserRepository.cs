using RegistrationSystemWithFramework.Data;

namespace RegistrationSystemWithFramework.Repository
{
    public interface IUserRepository
    {
        public void Add(User user);
        public User FindByEmail(string email);
        public void Update(Models.UserUpdateViewModel userViewModel);
        public bool CodeVerification(string verificationCode);
        public bool ForgotPassword(string email);
        public bool ResetPassword(string token, string email, string password);

    }
}
