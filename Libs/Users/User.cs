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

        public UserPermissions GetUserPermissions()
        {
            List<string> claims = GetClaims();

            if (claims == null)
                claims=new List<string>();

            return new UserPermissions
            {
                DashboardObserver = claims.Contains("DashboardObserver"),
                DashboardEditor = claims.Contains("DashboardEditor"),
                EditorObserver = claims.Contains("EditorObserver"),
                EditorEditor = claims.Contains("EditorEditor"),
                HardwareObserver = claims.Contains("HardwareObserver"),
                LogsObserver = claims.Contains("LogsObserver"),
                LogsEditor = claims.Contains("LogsEditor"),
                ConfigObserver = claims.Contains("ConfigObserver"),
                ConfigEditor = claims.Contains("ConfigEditor"),
                UsersObserver = claims.Contains("UsersObserver"),
                UsersEditor = claims.Contains("UsersEditor"),
            };
        }

        public void SetClaims(UserPermissions userPermissions)
        {
            List<string> claims = new List<string>();
        
            if (userPermissions.DashboardObserver)
                claims.Add("DashboardObserver");
            if (userPermissions.DashboardEditor)
                claims.Add("DashboardEditor");
            if (userPermissions.EditorObserver)
                claims.Add("EditorObserver");
            if (userPermissions.EditorEditor)
                claims.Add("EditorEditor");
            if (userPermissions.HardwareObserver)
                claims.Add("HardwareObserver");
            if (userPermissions.LogsObserver)
                claims.Add("LogsObserver");
            if (userPermissions.LogsEditor)
                claims.Add("LogsEditor");
            if (userPermissions.ConfigObserver)
                claims.Add("ConfigObserver");
            if (userPermissions.ConfigEditor)
                claims.Add("ConfigEditor");
            if (userPermissions.UsersObserver)
                claims.Add("UsersObserver");
            if (userPermissions.UsersEditor)
                claims.Add("UsersEditor");

            SetClaims(claims);
        }
    }
}
