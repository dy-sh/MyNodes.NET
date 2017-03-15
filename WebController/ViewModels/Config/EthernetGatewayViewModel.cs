using System.ComponentModel.DataAnnotations;

namespace MyNodes.WebController.ViewModels.Config
{
    public class EthernetGatewayViewModel
    {
        [Required]
        [Display(Name = "IP")]
        public string Ip { get; set; }

        [Required]
        [Display(Name = "Port")]
        public int Port { get; set; }
    }
}
