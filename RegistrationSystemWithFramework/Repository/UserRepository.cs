using Microsoft.EntityFrameworkCore;
using RegistrationSystemWithFramework.Data;

namespace RegistrationSystemWithFramework.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public void Add(User user)
        {
            _context.Users.Add(user);

            _context.SaveChanges();
        }

        public User FindByEmail(string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);

            return user;
        }

        public void Update(Models.UserUpdateViewModel userViewModel)
        {
            var entity = FindByEmail(userViewModel.Email);

            entity.Password = userViewModel.Password;
            entity.Name = userViewModel.Name;

            _context.Users.Update(entity);
            _context.SaveChanges();
        }

        public bool CodeVerification(string verificationCode)
        {
            var user = _context.Users.FirstOrDefault(u => u.VerificationCode == verificationCode);
            if (user != null)
            {
                user.IsVerified = true;
                user.VerificationCode = null;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ForgotPassword(string email)
        {
            User user = FindByEmail(email);
            if (user != null)
            {
                user.resetPassword = Guid.NewGuid().ToString();
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ResetPassword(string token, string email, string password)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == email && u.resetPassword == token);
            if (user != null)
            {
                user.Password = password;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
