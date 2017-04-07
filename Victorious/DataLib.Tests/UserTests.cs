using System;
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
        public void Test_UsernameExists()
        {
            var db = new DatabaseInterface();
            
        }

        [TestMethod]
        public void Test_AddUser_and_UsernameExists()
        {
            var dbInterface = new DatabaseInterface();
            var user = new UserModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = "keltonr01",
                Password = "1234",
                SitePermission = new PermissionModel()
                {
                    Permission = Permission.SITE_ADMINISTRATOR
                }
            };
            if (dbInterface.UserUsernameExists(user.Username) == DbError.DOES_NOT_EXIST)
            {
                user.Username = "keltonr02";
                dbInterface.AddUser(user); 
            }

            var result = dbInterface.GetUserByUsername("keltonr01");
            Assert.AreEqual(user, result);
        }
    }
}
