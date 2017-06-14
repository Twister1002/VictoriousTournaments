using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;
using WebApplication.Models.ViewModels;
using WebApplication.Utility;
using WebApplication.Interfaces;

namespace WebApplication.Models
{
    public class Admin : Model
    {
        public GameType gameType { get; private set; }
        public BracketType bracketTypes { get; private set; }
        public Platform platformTypes { get; private set; }

        public Admin(IService service) : base(service)
        {
            Init();
        }

        private void Init()
        {
            // Load the lists
            gameType = new GameType(services);
            bracketTypes = new BracketType(services);
            platformTypes = new Platform(services);
        }

        #region Helpers

        #endregion
    }
}