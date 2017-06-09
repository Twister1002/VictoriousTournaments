using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class EmailTests
    {
        IUnitOfWork unitOfWork;
        EmailService emailService;
        AccountService accountService;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            emailService = new EmailService(unitOfWork);
            accountService = new AccountService(unitOfWork);
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
    }
}
