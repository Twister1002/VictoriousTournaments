using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class UserModel
    {
        public String Username { get; private set; }
        private String Password;
        public String Email { get; set; }

        public UserModel(AccountRegisterViewModel model)
        {
            Username = model.Username;
            Password = model.Password;
            Email = model.Email;
        }
    }
}