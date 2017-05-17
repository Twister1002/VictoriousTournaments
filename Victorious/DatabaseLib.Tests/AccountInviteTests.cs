using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class AccountInviteTests
    {
        [TestMethod]
        public void Add_Account_Invite()
        {
            var db = new DbInterface();

            AccountInviteModel invite = new AccountInviteModel()
            {
                DateCreated = DateTime.Now,
                DateExpires = DateTime.Now.AddDays(1),
                IsExpired = false,
                AccountInviteCode = "10000",
                SentToEmail = "keltonr01@gmail.com",
                SentByID = 1
            };

            var result = db.AddAccountInvite(invite);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Update_Account_Invite()
        {
            var db = new DbInterface();

            AccountInviteModel invite = db.GetAcountInvite("10000");
            invite.IsExpired = true;
            var result = db.UpdateAccountInvite(invite);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Delete_Account_Invite()
        {
            var db = new DbInterface();

            var result = db.DeleteAccountInvite("10000");

            Assert.AreEqual(DbError.SUCCESS, result);
        }



    }
}
