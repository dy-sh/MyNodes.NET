using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyNetSensors.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ClaimsJson { get; set; }

        public List<string> GetClaims()
        {
            if (String.IsNullOrEmpty(ClaimsJson))
                return null;

            return JsonConvert.DeserializeObject<List<string>>(ClaimsJson);
        }

        public void SetClaims(List<string> claims)
        {
            if (claims == null || claims.Count == 0)
                ClaimsJson = null;

            ClaimsJson = JsonConvert.SerializeObject(claims);
        }

        public static List<string> GetAllClaims()
        {
            return new List<string>
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
