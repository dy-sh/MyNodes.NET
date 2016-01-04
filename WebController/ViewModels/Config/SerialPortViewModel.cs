using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.ViewModels.Config
{
    public class SerialPortViewModel
    {
        [Required]
        [Display(Name = "Serial Port")]
        public string PortName { get; set; }
    }
}
