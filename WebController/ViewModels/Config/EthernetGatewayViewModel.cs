using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.ViewModels.Config
{
    public class EthernetGatewayViewModel
    {
        [Required]
        [Display(Name = "Gateway IP")]
        public string Ip { get; set; }

        [Required]
        [Display(Name = "Gateway Port")]
        public int Port { get; set; }
    }
}
