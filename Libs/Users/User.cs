using System;
using System.Collections.Generic;

namespace MyNetSensors.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Claims { get; set; }
    }
}
