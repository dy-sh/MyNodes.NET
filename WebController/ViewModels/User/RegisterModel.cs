using System.ComponentModel.DataAnnotations;

namespace MyNodes.WebController.ViewModels.User
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Login is required")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
