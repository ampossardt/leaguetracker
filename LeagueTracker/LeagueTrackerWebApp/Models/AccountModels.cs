using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Engine.Data.Types;

namespace LeagueTrackerWebApp.Models
{
    public class LoginModel
    {
        [Required]
        [MaxLength(15)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public RegisterModel()
        {
            UserTypes = new Dictionary<string, string>
            {
                { "Owner", "Owner or commissioner of a league." },
                { "Coach", "Coach of a team in an existing league." }
            };
        }

        [Required(ErrorMessage = "Please enter your first name.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter an email address.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,6}$", ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a user name.")]
        [RegularExpression(@"^[a-zA-Z0-9]{5,15}$", ErrorMessage = "Please enter a valid username. Valid usernames contain alphanumeric characters only and are 5 to 15 characters long.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [RegularExpression(@".{5,20}", ErrorMessage = "Passwords must be between 5 and 20 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You must confirm your password.")]
        [Compare("Password", ErrorMessage = "Passwords must match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "")]
        public string UserType { get; set; }

        public Dictionary<string, string> UserTypes { get; set; }
    }
}
