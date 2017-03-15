using System.ComponentModel.DataAnnotations;

namespace MyNodes.WebController.ViewModels.FirstRun
{
    public class ExternalDatabaseViewModel
    {
        [Required]
        [Display(Name = "Connection String")]
        public string ConnectionString { get; set; }
    }
}
