using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNodes.WebController.ViewModels.Config
{
    public class SerialGatewayViewModel
    {
        [Required]
        [Display(Name = "Serial Port Name")]
        public string PortName { get; set; }

        [Required]
        [Display(Name = "Baud Rate")]
        public int Boudrate { get; set; } = 115200;
    }
}
