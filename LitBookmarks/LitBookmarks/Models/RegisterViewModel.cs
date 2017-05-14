using System.ComponentModel.DataAnnotations;

namespace LitBookmarks.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter a username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        public string PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Please enter a email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a firstname")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter a lastname")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Please enter some information about you")]
        public string About { get; set; }
    }
}