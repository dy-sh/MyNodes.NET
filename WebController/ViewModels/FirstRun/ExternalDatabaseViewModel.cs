using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.ViewModels.FirstRun
{
    public class ExternalDatabaseViewModel
    {
        [Required]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }
    }
}
