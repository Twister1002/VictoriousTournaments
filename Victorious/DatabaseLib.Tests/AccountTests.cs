﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var account = db.GetAccountByUsername("keltonr01");
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

            var result = db.DeleteAccount(db.GetAccountById(user.AccountID));

            Assert.AreEqual(DbError.SUCCESS, result);
        }


    }
}
