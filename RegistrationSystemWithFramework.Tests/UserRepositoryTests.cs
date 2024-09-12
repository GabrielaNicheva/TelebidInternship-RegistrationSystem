using Microsoft.EntityFrameworkCore;
using RegistrationSystemWithFramework.Data;
using RegistrationSystemWithFramework.Models;
using RegistrationSystemWithFramework.Repository;

namespace RegistrationSystemWithFramework.Tests
{
    public class Tests
    {
        private UserRepository userRepository;

        private ApplicationDbContext applicationDbContext;

        [SetUp]
        public void SetUp()
        {
            applicationDbContext = ApplicationContextSetUp();

            userRepository = new UserRepository(applicationDbContext);
        }

        [TearDown]
        public void TearDown()
        {
            applicationDbContext.Database.EnsureDeleted();
            applicationDbContext.Dispose();
        }

        #region Add
        [Test]
        public void GivenAnUserWithNonNullableValues_WhenAddingUser_AddsUser()
        {
            var user = new User("name", "email", "password", "gender", "isocode", "phone", "address", true, "111111");

            userRepository.Add(user);

            var createdUser = applicationDbContext.Users.LastOrDefault();

            Assert.NotNull(createdUser, "User is null");
            Assert.That(createdUser.Name, Is.EqualTo(user.Name), "User name is different than expected");
            Assert.That(createdUser.Email, Is.EqualTo(user.Email), "User email is different than expected");
            Assert.That(createdUser.Password, Is.EqualTo(user.Password), "User password is different than expected");
            Assert.That(createdUser.Gender, Is.EqualTo(user.Gender), "User gender is different than expected");
            Assert.That(createdUser.ISOCode, Is.EqualTo(user.ISOCode), "User ISO code is different than expected");
            Assert.That(createdUser.Phone, Is.EqualTo(user.Phone), "User phone is different than expected");
            Assert.That(createdUser.Address, Is.EqualTo(user.Address), "User address is different than expected");
        }

        [Test]
        public void GivenAnUserWithNullableValues_WhenAddingUser_AddsUser()
        {
            var user = new User("name", "email", "password", "", "isocode", "phone", "", true, "111111");

            userRepository.Add(user);

            var createdUser = applicationDbContext.Users.LastOrDefault();

            Assert.NotNull(createdUser, "User is null");
            Assert.That(createdUser.Name, Is.EqualTo(user.Name), "User name is different than expected");
            Assert.That(createdUser.Email, Is.EqualTo(user.Email), "User email is different than expected");
            Assert.That(createdUser.Password, Is.EqualTo(user.Password), "User password is different than expected");
            Assert.That(createdUser.Gender, Is.EqualTo(user.Gender), "User gender is different than expected");
            Assert.That(createdUser.ISOCode, Is.EqualTo(user.ISOCode), "User ISO code is different than expected");
            Assert.That(createdUser.Phone, Is.EqualTo(user.Phone), "User phone is different than expected");
            Assert.That(createdUser.Address, Is.EqualTo(user.Address), "User address is different than expected");
        }
        #endregion


        #region FinByEmail
        [Test]
        public void GivenAnExistingEmail_WhenGettingAUser_ReturnsTheUser()
        {
            var expectedUsers = SeedUsers();
            var expectedUser = expectedUsers.First();

            var user = userRepository.FindByEmail(expectedUser.Email);

            Assert.That(user, Is.EqualTo(expectedUser), "User is different than expected");
        }

        [Test]
        public void GivenNonExistingEmail_WhenGettingAUser_ReturnsNull()
        {
            User expectedUser = null;

            var user = userRepository.FindByEmail("...");

            Assert.That(user, Is.EqualTo(expectedUser), "User is not null");
        }

        #endregion

        #region Update
        [Test]
        public void Update_ExistingUser_UpdatesUser()
        {
            var userViewModel = new UserUpdateViewModel { Email = "email1@test.com", Password = "newpassword", Name = "New Name" };
            var expectedUsers = SeedUsers();
            userRepository.Update(userViewModel);

            var updatedUser = expectedUsers.FirstOrDefault(u => u.Email == userViewModel.Email);
            Assert.That(updatedUser.Password, Is.EqualTo(userViewModel.Password));
            Assert.That(updatedUser.Name, Is.EqualTo(userViewModel.Name));
        }
        #endregion

        #region CodeVerification
        #endregion
        private IEnumerable<User> SeedUsers()
        {
            var users = new[]
            {
                new User( "name1", "email1@test.com", "password1", "gender1", "isocode1","phone1", "address", true, "111111"),
                new User( "name2", "email2@test.com", "password2", "gender1", "isocode2","phone2", "address", true, "111111"),
                new User( "name3", "email3@test.com", "password3", "", "isocode3","phone3", "",  true, "111111"),
            };

            applicationDbContext.Users.AddRange(users);
            applicationDbContext.SaveChanges();

            return users;
        }
        private ApplicationDbContext ApplicationContextSetUp()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("UnitTestsDb");

            return new ApplicationDbContext(options.Options);
        }
    }


}