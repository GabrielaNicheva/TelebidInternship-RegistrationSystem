namespace RegistrationSystemWithFramework.Models
{
    public class UserUpdateViewModel
    {
        public string Email { get; set; }

        public string Name {  get; set; }
        public string Password { get; set; }

        public UserUpdateViewModel(string email,string name, string password)
        {
            Email = email;
            Name = name;
            Password = password;
        }

        public UserUpdateViewModel()
        {
            
        }
    }
}
