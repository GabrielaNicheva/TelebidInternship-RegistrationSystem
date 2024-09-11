using System.ComponentModel.DataAnnotations;

namespace RegistrationSystemWithFramework.Data
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string? Gender { get; set; }

        [Required]
        public string ISOCode { get; set; }

        [Required]
        public string Phone { get; set; }
        public string? Address { get; set; }

        public User()
        {
            
        }

        public User(string name, string email, string password, string gender, string isocode, string phone, string address)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Email = email;
            Password = password;
            Gender = gender;
            ISOCode = isocode;
            Phone = phone;
            Address = address;
        }
    }
}
