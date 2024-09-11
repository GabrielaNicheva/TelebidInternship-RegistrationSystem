using System.ComponentModel.DataAnnotations;

namespace RegistrationSystemWithFramework.Models
{
    public class UserRegistrationViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }
        public string? Gender { get; set; }
        public string ISOCode { get; set; }
        public string Phone { get; set; }
        public string? Address { get; set; }
    }
}
