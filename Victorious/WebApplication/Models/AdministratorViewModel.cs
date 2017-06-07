using System;
using System.Collections.Generic;
using DatabaseLib;
using WebApplication.Models.Administrator;
using WebApplication.Utility;

namespace WebApplication.Models
{
    public class AdministratorViewModel : ViewModel
    {
        public AdministratorViewModel(Service services) : base(services)
        {

        }

        public GameTypeViewModel LoadGameTypes()
        {
            return new GameTypeViewModel(services);
        }

        public PlatformTypeViewModel LoadPlatformTypes()
        {
            return new PlatformTypeViewModel(services);
        }

        public BracketTypeViewModel LoadBracketTypes()
        {
            return new BracketTypeViewModel(services);
        }
    }
}