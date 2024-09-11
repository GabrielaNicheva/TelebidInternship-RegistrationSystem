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

    }
}
