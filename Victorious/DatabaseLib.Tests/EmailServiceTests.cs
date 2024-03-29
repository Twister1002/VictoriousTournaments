﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class EmailServiceTests
    {
        IUnitOfWork unitOfWork;
        EmailService emailService;
        AccountService accountService;
        TournamentService tournamentService;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            emailService = new EmailService(unitOfWork);
            accountService = new AccountService(unitOfWork);
            tournamentService = new TournamentService(unitOfWork);
        }

        #region AccountInvite Emails

        [TestMethod]
        [TestCategory("Email Service")]
        public void AccountInviteEmail()
        {
            var result = emailService.SendAccountInviteEmail(accountService.GetAllAccountInvites()[0]);

            Assert.AreEqual(true, result);
        }

        #endregion

        #region TournamentInvite Emails

        [TestMethod]
        [TestCategory("Email Service")]
        public void SendTournamentInviteEmail()
        {
            var recepiants = new List<string>()
            {
                "keltonr01@gmail.com"
            };
            var tournamentInvite = tournamentService.GetAllTournamentInvites()[0];
            var result = emailService.SendTournamentInviteEmail(tournamentInvite, "http://localhost:20346/Tournament/" + tournamentInvite.TournamentID, recepiants);

            Assert.AreEqual(true, result);
        }

        #endregion

        #region MailingList

        [TestMethod]
        [TestCategory("Email Service")]
        public void AddEmailToMailingList()
        {
            string email = "keltonr01@gmail.com";
            emailService.AddEmailToMailingList(email);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Email Service")]
        public void GetMailingList()
        {
            List<string> mailingList = emailService.GetMailingList();

            Assert.AreNotEqual(0, mailingList.Count);
        }

        [TestMethod]
        [TestCategory("Email Service")]
        public void RemoveEmailFromMailingList()
        {
            string email = "keltonr01@gmail.com";
            emailService.RemoveEmailFromMailingList(email);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Email Service")]
        public void SendEmailToMailingList()
        {
            var result = emailService.SendEmailToMailingList("mailing list test", "new site update tests");

            Assert.AreEqual(true, result);
        }

        #endregion

    }
}
