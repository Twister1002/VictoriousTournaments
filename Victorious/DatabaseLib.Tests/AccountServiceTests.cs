using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Linq;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        IUnitOfWork unitOfWork;
        AccountService service;
        
        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new AccountService(unitOfWork);
        }

        #region Accounts

        [TestMethod]
        [TestCategory("Account Service")]
        public void AddUserAccount()
        {
            var user = new AccountModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                //Username = Guid.NewGuid().ToString(),
                Username = "keltonr01",
                Email = "keltonr01@gmail.com",
                Password = "1234",
                PermissionLevel = (int)Permission.SITE_STANDARD
            };

            service.AddAccount(user);

            var result = unitOfWork.Save();
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void AccountUsernameExists_Exists()
        {
            var result = service.AccountUsernameExists("keltonr01");
            Assert.AreEqual(DbError.EXISTS, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void AccountUsernameExists_Does_Not_Exists()
        {
            var result = service.AccountUsernameExists("keltonr02");
            Assert.AreEqual(DbError.DOES_NOT_EXIST, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void AccountEmailExists_Exists()
        {
            var result = service.AccountEmailExists("keltonr01@gmail.com");
            Assert.AreEqual(DbError.EXISTS, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void AccountEmailExists_DoesNotExists()
        {
            var result = service.AccountEmailExists("keltonr02@gmail.com");
            Assert.AreEqual(DbError.DOES_NOT_EXIST, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void GetAccount_By_Id()
        {
            var account = service.GetAccount(2);
            var result = account.Email;
            Assert.AreEqual("keltonr01@gmail.com", result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void GetAccount_By_Usernament()
        {
            var account = service.GetAccount("keltonr01");
            var result = account.Email;
            Assert.AreEqual("keltonr01@gmail.com", result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void UpdateAccount_Save()
        {
            var account = service.GetAccount("keltonr01");
            account.FirstName = "Billy";
            service.UpdateAccount(account);
            unitOfWork.Save();
            account = service.GetAccount("keltonr01");
            var result = account.FirstName;

            Assert.AreEqual("Billy", result);

        }

        [TestMethod]
        public void GetTournamentsForAccount()
        {
            List<TournamentModel> accountTournaments = service.GetTournamentsForAccount(1);
            List<TournamentModel> tournaments = unitOfWork.TournamentRepo.GetAll().ToList();
            var result = (!tournaments.Except(accountTournaments).Any() && !accountTournaments.Except(tournaments).Any());
            Assert.AreEqual(true, result);
        }

        #endregion


        #region AccountInvites

        [TestMethod]
        [TestCategory("Account Service")]
        public void AddAccountInvite()
        {
            AccountInviteModel invite = new AccountInviteModel()
            {
                DateCreated = DateTime.Today,
                DateExpires = DateTime.Today.AddDays(1),
                AccountInviteCode = "1234",
                IsExpired = false,
                SentByID = 2,
                SentToEmail = "keltonr01@gmail.com"
            };
            service.AddAccountInvite(invite);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void GetAccountInvite_Returns_Invite()
        {
            var invite = service.GetAccountInvite("1234");

            Assert.AreEqual(DateTime.Today, invite.DateCreated);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void AccountInvite_Returns_Null()
        {
            var invite = service.GetAccountInvite("1235");

            Assert.IsNull(invite);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void UpdateAccountInvite()
        {
            var invite = service.GetAccountInvite("1234");
            var expected = true;
            invite.IsExpired = expected;
            service.UpdateAccountInvite(invite);
            unitOfWork.Save();
            invite = service.GetAccountInvite("1234");
            var result = invite.IsExpired;

            Assert.AreEqual(expected, result);
             
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void DeleteAccountInvite_Save()
        {
            service.DeleteAccountInvite("1234");
            unitOfWork.Save();
            var result = service.GetAccountInvite("1234");

            Assert.IsNull(result);

        }

        #endregion

    }

}
