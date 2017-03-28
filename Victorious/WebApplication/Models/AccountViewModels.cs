using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AccountViewModel : AccountFields
    {
        public UserModel Model { get; private set; }

        public AccountViewModel()
        {
            Model = new UserModel();
        }

        public AccountViewModel(int id)
        {
            Model = db.GetUserById(id);
            Init();
        }

        public AccountViewModel(UserModel model)
        {
            Model = model;
            Init();
        }

        private void Init()
        {
            List<TournamentModel> tournies = db.GetAllTournaments();
            Model.Tournaments = tournies.Where(t => t.CreatedByID == Model.UserID).ToList();
        }

        public override void ApplyChanges()
        {
            // Non null fields
            Model.Username      = this.Username != String.Empty ? this.Username : String.Empty;
            Model.Email         = this.Email != String.Empty ? this.Email : String.Empty;

            // Null fields
            Model.FirstName     = this.FirstName;
            Model.LastName      = this.LastName;
            Model.Password      = this.Password;
        }

        public override void SetFields()
        {
            this.Username   = Model.Username;
            this.Email      = Model.Email;
            this.LastName   = Model.LastName;
            this.FirstName  = Model.FirstName;
        }

        public void setUserModel()
        {
            if (Model != null)
            {
                Model = db.GetUserById(Model.UserID);
            }
        }

        public void setUserModel(int id)
        {
            if (id > 0)
            {
                Model = db.GetUserById(id);
            }
        }

        public void setUserModel(String name)
        {
            if (name != String.Empty)
            {
                Model = db.GetUserByUsername(name);
            }
        }
    }
}