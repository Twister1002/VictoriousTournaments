using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AccountViewModel : AccountFields
    {
        private UserModel userModel;

        public AccountViewModel()
        {
            userModel = new UserModel();
        }

        public AccountViewModel(int id)
        {
            userModel = db.GetUserById(id);
            Init();
        }

        public AccountViewModel(UserModel model)
        {
            userModel = model;
            Init();
        }

        private void Init()
        {
            List<TournamentModel> tournies = db.GetAllTournaments();
            userModel.Tournaments = tournies.Where(t => t.CreatedByID == userModel.UserID).ToList();
        }

        public void ApplyFieldChanges()
        {
            userModel.Username      = this.Username;
            userModel.FirstName     = this.FirstName;
            userModel.LastName      = this.LastName;
            userModel.Email         = this.Email;
            userModel.Password      = this.Password;
        }

        public AccountFields SetFields()
        {
            this.Username   = userModel.Username;
            this.Email      = userModel.Email;
            this.LastName   = userModel.LastName;
            this.FirstName  = userModel.FirstName;

            return this;
        }

        public UserModel getUserModel()
        {
            return userModel;
        }

        public void setUserModel()
        {
            if (userModel != null)
            {
                userModel = db.GetUserById(userModel.UserID);
            }
        }
        public void setUserModel(int id)
        {
            if (id > 0)
            {
                userModel = db.GetUserById(id);
            }
        }
        public void setUserModel(String name)
        {
            if (name != String.Empty)
            {
                userModel = db.GetUserByUsername(name);
            }
        }
    }
}