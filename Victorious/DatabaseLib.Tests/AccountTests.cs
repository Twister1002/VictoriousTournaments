using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void Add_User_Acccount()
        {
            var db = new DbInterface();
            var user = new AccountModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                PermissionLevel = (int)Permission.SITE_STANDARD,

               
            };

            var result = db.AddAccount(user);
            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Username_Exists_Exists()
        {
            var db = new DbInterface();

            var result = db.AccountUsernameExists("keltonr01");

            Assert.AreEqual(DbError.EXISTS, result);
        }

        [TestMethod]
        public void Username_Does_Not_Exist()
        {
            var db = new DbInterface();

            var result = db.AccountUsernameExists("keltonr02");

            Assert.AreEqual(DbError.DOES_NOT_EXIST, result);
        }

        [TestMethod]
        public void Get_Account_By_Username()
        {
            var db = new DbInterface();

            var account = db.GetAccount("keltonr01");
            var result = false;
            if (account.Email == "keltonr01@gmail.com" && account.PhoneNumber.Contains("9542534919"))
                result = true;

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Delete_Account()
        {
            var db = new DbInterface();

            var user = new AccountModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                PermissionLevel = (int)Permission.SITE_STANDARD,
            };

            db.AddAccount(user);

            var result = db.DeleteAccount(db.GetAccount(user.AccountID));

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Tournaments_For_Account()
        {
            var db = new DbInterface();

            List<TournamentModel> accountTournaments = db.GetTournamentsForAccount(1);
            List<TournamentModel> tournaments = db.GetAllTournaments().GetRange(0, 2);
            var result = (!tournaments.Except(accountTournaments).Any() && !accountTournaments.Except(tournaments).Any());
            Assert.AreEqual(true, result);
        }

    }
}
