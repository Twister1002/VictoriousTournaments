﻿using System;
using System.Collections.Generic;
using DatabaseLib;
using WebApplication.Models.Administrator;

namespace WebApplication.Models
{
    public class AdministratorViewModel : ViewModel
    {
        public GameTypeViewModel LoadGameTypes()
        {
            return new GameTypeViewModel();
        }
    }
}