using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class EmailTests
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
        public void SendTournamentInviteEmail()
        {
            var recepiants = new List<string>()
            {
                "keltonr01@gmail.com"
            };
            var result = emailService.SendTournamentInviteEmail(tournamentService.GetAllTournamentInvites()[0], "http://localhost:20346/Tournament/5002", recepiants);

            Assert.AreEqual(true, result);
        }

        #endregion

    }
}
