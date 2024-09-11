namespace RegistrationSystemWithFramework.Models
{
    public class UserLoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public UserLoginViewModel(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public UserLoginViewModel()
        {
            
        }
    }
}
