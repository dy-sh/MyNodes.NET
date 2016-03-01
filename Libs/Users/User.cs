/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyNodes.Users
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
                UserClaims.DashboardObserver,
                UserClaims.DashboardEditor,
                UserClaims.EditorObserver,
                UserClaims.EditorEditor,
                UserClaims.EditorProtectedAccess,
                UserClaims.HardwareObserver,
                UserClaims.LogsObserver,
                UserClaims.LogsEditor,
                UserClaims.ConfigObserver,
                UserClaims.ConfigEditor,
                UserClaims.UsersObserver,
                UserClaims.UsersEditor,
            };
        }

        public UserPermissions GetUserPermissions()
        {
            List<string> claims = GetClaims();

            if (claims == null)
                claims=new List<string>();

            return new UserPermissions
            {
                DashboardObserver = claims.Contains(UserClaims.DashboardObserver),
                DashboardEditor = claims.Contains(UserClaims.DashboardEditor),
                EditorObserver = claims.Contains(UserClaims.EditorObserver),
                EditorEditor = claims.Contains(UserClaims.EditorEditor),
                EditorProtectedAccess = claims.Contains(UserClaims.EditorProtectedAccess),
                HardwareObserver = claims.Contains(UserClaims.HardwareObserver),
                LogsObserver = claims.Contains(UserClaims.LogsObserver),
                LogsEditor = claims.Contains(UserClaims.LogsEditor),
                ConfigObserver = claims.Contains(UserClaims.ConfigObserver),
                ConfigEditor = claims.Contains(UserClaims.ConfigEditor),
                UsersObserver = claims.Contains(UserClaims.UsersObserver),
                UsersEditor = claims.Contains(UserClaims.UsersEditor),
            };
        }

        public void SetClaims(UserPermissions userPermissions)
        {
            List<string> claims = new List<string>();
        
            if (userPermissions.DashboardObserver)
                claims.Add(UserClaims.DashboardObserver);
            if (userPermissions.DashboardEditor)
                claims.Add(UserClaims.DashboardEditor);
            if (userPermissions.EditorObserver)
                claims.Add(UserClaims.EditorObserver);
            if (userPermissions.EditorEditor)
                claims.Add(UserClaims.EditorEditor);
            if (userPermissions.EditorProtectedAccess)
                claims.Add(UserClaims.EditorProtectedAccess);
            if (userPermissions.HardwareObserver)
                claims.Add(UserClaims.HardwareObserver);
            if (userPermissions.LogsObserver)
                claims.Add(UserClaims.LogsObserver);
            if (userPermissions.LogsEditor)
                claims.Add(UserClaims.LogsEditor);
            if (userPermissions.ConfigObserver)
                claims.Add(UserClaims.ConfigObserver);
            if (userPermissions.ConfigEditor)
                claims.Add(UserClaims.ConfigEditor);
            if (userPermissions.UsersObserver)
                claims.Add(UserClaims.UsersObserver);
            if (userPermissions.UsersEditor)
                claims.Add(UserClaims.UsersEditor);

            SetClaims(claims);
        }
    }
}
