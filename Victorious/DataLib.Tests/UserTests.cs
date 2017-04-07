﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataLib;
using System.Data.Entity;

namespace DataLib.Tests
{
    [TestClass]
    public class UserTests
    {
        //private VictoriousDbContext context;

        [ClassInitialize]
        void TestSetup()
        {
            //context = new VictoriousDbContext();
            //if (context.Database.Exists())
            //{
            //    context.Database.Delete();
            //}
            //context.Database.Create();
        }

        [TestMethod]
        public void Username_Exists()
        {
            var db = new DatabaseInterface();
            var result = db.UserUsernameExists("keltonr01");

            Assert.AreEqual(DbError.EXISTS, result);
        }

        [TestMethod]
        public void Username_Does_Not_Exist()
        {
            var db = new DatabaseInterface();
            var result = db.UserUsernameExists("asdf");

            Assert.AreEqual(DbError.DOES_NOT_EXIST, result);
        }

        [TestMethod]
        public void Test_AddUser()
        {
            var db = new DatabaseInterface();
            var user = new UserModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                SitePermission = new PermissionModel()
                {
                    Permission = Permission.SITE_ADMINISTRATOR
                }
            };
            
            var result = db.AddUser(user);
            Assert.AreEqual(DbError.SUCCESS, result);
        }
    }
}
