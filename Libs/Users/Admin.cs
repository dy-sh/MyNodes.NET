using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Users
{
    public class Admin : User
    {
        public Admin()
        {
            Name = "Admin";
            Password = "Admin";

            Claims = new List<string>
            {
                "DashboardObserver",
                "DashboardEditor",
                "EditorObserver",
                "EditorEditor",
                "HardwareObserver",
                "LogsObserver",
                "LogsEditor",
                "ConfigObserver",
                "ConfigEditor",
                "UsersObserver",
                "UsersEditor"
            };
        }
    }
}
