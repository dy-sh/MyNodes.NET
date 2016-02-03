/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Users
{
    public interface IUsersRepository
    {
        string AddUser(User user);
        void UpdateUser(User user);
        User GetUser(string id);
        List<User> GetAllUsers();
        void RemoveUser(string id);
        void RemoveAllUsers();
    }
}
