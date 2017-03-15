using System.ComponentModel.DataAnnotations;

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
