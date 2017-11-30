﻿using DatabaseLib;
using DatabaseLib.Services;
using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication.Interfaces;

namespace WebApplication.Utility
{
    public class Service : IService
    {
        public Exception e { get; set; }
        private IUnitOfWork work;
        public AccountService Account { get; private set; }
        public TournamentService Tournament { get; private set; }
        public TypeService Type { get; private set; } 
        public FacebookClient FBClient { get; private set; }
        //public EmailService Email { get; private set; }

        public Service(IUnitOfWork work)
        {
            this.work = work;
            Account = new AccountService(work);
            Tournament = new TournamentService(work);
            Type = new TypeService(work);
            //Email = new EmailService(work);
            FBClient = new FacebookClient();
            FBClient.AppId = Properties.Settings.Default.FacebookID;
            FBClient.AppSecret = Properties.Settings.Default.FacebookSecret;
        }

        public bool Save()
        {
            if (work.Save())
            {
                return true;
            }
            else
            {
                e = work.GetException();
                work.Refresh();
                return false;
            }
        }

        public Exception GetException()
        {
            return e;
        }
    }
}