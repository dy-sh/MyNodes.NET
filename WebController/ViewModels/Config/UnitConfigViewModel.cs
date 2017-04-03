using System.ComponentModel.DataAnnotations;

namespace MyNodes.WebController.ViewModels.Config
{
    public class UnitConfigViewModel
    {
        [Required]
        public bool IsMetric { get; set; }
    }
}
