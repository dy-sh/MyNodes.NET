using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.ViewModels.User
{
    public class NewUserModel
    {
        [Required(ErrorMessage = "Login is required")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
