using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Engine.Data.Types;

namespace LeagueTrackerWebApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        [MaxLength(15)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [MaxLength(20)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ResetModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        public string Email { get; set; }
    }
}
