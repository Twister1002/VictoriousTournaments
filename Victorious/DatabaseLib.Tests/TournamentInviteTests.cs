using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TournamentInviteTests
    {
        [TestMethod]
        public void Add_TournamentInvite()
        {
            var db = new DbInterface();

            TournamentInviteModel invite = new TournamentInviteModel()
            {
                DateCreated = DateTime.Today,
                DateExpires = DateTime.Today.AddDays(1),
                IsExpired = false,
                TournamentInviteCode = "30000",
                NumberOfUses = 0,
                TournamentID = 1  
            };
            var result = db.AddTournamentInvite(invite);

            Assert.AreEqual(DbError.SUCCESS, result);
            
        }

        [TestMethod]
        public void Get_Tournament_Invite()
        {
            var db = new DbInterface();

            TournamentInviteModel invite = db.GetTournamentInvite("10000");

            Assert.AreEqual("10000", invite.TournamentInviteCode);
        }

        [TestMethod]
        public void Update_Tournament_Invite()
        {
            var db = new DbInterface();

            TournamentInviteModel invite = db.GetTournamentInvite("10000");
            invite.IsExpired = true;
            var result = db.UpdateTournamentInvite(invite);

            Assert.AreEqual(result, DbError.SUCCESS);
        }

        [TestMethod]
        public void DeleteTournamentInvite()
        {
            var db = new DbInterface();

            var result = db.DeleteTournamentInvite("30000");

            Assert.AreEqual(DbError.SUCCESS, result);
        }

    }
}
