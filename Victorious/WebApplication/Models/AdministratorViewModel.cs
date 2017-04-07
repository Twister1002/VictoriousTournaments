using DataLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class AdministratorViewModel : AdministratorFields
    {
        public List<GameTypeModel> Games { get; private set; }

        public AdministratorViewModel()
        {
            Games = db.GetAllGameTypes();
        }
    }
}