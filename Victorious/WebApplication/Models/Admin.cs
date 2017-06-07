using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using WebApplication.Models.ViewModels;

namespace WebApplication.Models
{
    public class Admin : Model
    {
        AdminViewModel viewModel;

        public Admin(IUnitOfWork work) : base(work)
        {

        }

        #region CRUD

        #endregion

        #region Helpers

        #endregion
    }
}