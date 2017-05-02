using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLib.Tests
{
    [TestClass]
    public class PermissionTests
    {
       
        [TestInitialize]
        void TestSetup()
        {
           
        }

        [TestMethod]
        public void Get_User_Tournament_Permission()
        {
            var db = new DatabaseInterface();
            var user = db.GetAllUsers()[0];
            var tournament = db.GetAllTournaments()[0];

            var result = db.GetUserTournamentPermission(user, tournament);

            Assert.AreEqual(Permission.TOURNAMENT_ADMINISTRATOR, result);
            
        }

        [TestMethod]
        public void Get_User_Site_Permission()
        {
            var db = new DatabaseInterface();
            var user = db.GetAllUsers()[0];
            var tournament = db.GetAllTournaments()[0];

            var result = db.GetUserSitePermission(user);

            Assert.AreEqual(Permission.SITE_ADMINISTRATOR, result);

        }

        [TestMethod]
        public void Update_User_Tournament_Permission_Returns_Success()
        {
            var db = new DatabaseInterface();
            var user = db.GetAllUsers()[0];
            var tournament = db.GetAllTournaments()[0];

            var result = db.UpdateUserTournamentPermission(user, tournament, Permission.TOURNAMENT_ADMINISTRATOR);

            Assert.AreEqual(DbError.SUCCESS, result);

        }

        [TestMethod]
        public void Update_User_Site_Permission_Returns_Success()
        {
            var db = new DatabaseInterface();
            var user = db.GetAllUsers()[0];

            var result = db.UpdateUserSitePermission(user, Permission.SITE_ADMINISTRATOR);

            Assert.AreEqual(DbError.SUCCESS, result);

        }

    }
}
