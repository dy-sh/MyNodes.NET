/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Users
{
    public interface IUsersRepository
    {
        int AddUser(User user);
        void UpdateUser(User user);
        User GetUser(int id);
        User GetUser(string name);
        List<User> GetAllUsers();
        void RemoveUser(int id);
        void RemoveAllUsers();
        void RemoveUsers(List<User> users);
        int GetUsersCount();
    }
}
