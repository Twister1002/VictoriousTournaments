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
        //AdminViewModel viewModel;
        //public List<BracketTypeModel> Brackets { get; private set; }
        //public List<PlatformModel> Platforms { get; private set; }
        //public List<GameTypeModel> Games { get; private set; }

        public Admin(IUnitOfWork work) : base(work)
        {
            Init();
        }

        private void Init()
        {
            // Load the lists
            //Games = services.Type.GetAllGameTypes();
            //Brackets = services.Type.GetAllBracketTypes();
            //Platforms = services.Type.GetAllPlatforms();
        }

        #region Helpers

        #endregion
    }
}