using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.ViewModels.Config
{
    public class WebServerRules
    {
        public bool AllowFullAccessWithoutAuthorization { get; set; }
        public bool AllowRegistrationOfNewUsers { get; set; }
    
    }
}
