using RegistrationSystemWithFramework.Data;

namespace RegistrationSystemWithFramework.Repository
{
    public interface IUserRepository
    {
        public void Add(User user);
        public User FindByEmail(string email);
        public void Update(Models.UserUpdateViewModel userViewModel);
    }
}
