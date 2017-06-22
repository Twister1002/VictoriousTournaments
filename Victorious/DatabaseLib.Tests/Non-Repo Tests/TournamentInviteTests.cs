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
            var db = new DatabaseRepository("VictoriousEntities");

            TournamentInviteModel invite = new TournamentInviteModel()
            {
                DateCreated = DateTime.Today,
                DateExpires = DateTime.Today.AddDays(1),
                IsExpired = false,
                TournamentInviteCode = "10011",
                NumberOfUses = 0,
                TournamentID = 2  
            };
            var result = db.AddTournamentInvite(invite);

            Assert.AreEqual(DbError.SUCCESS, result);
            
        }

        [TestMethod]
        public void Get_Tournament_Invite()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            TournamentInviteModel invite = db.GetTournamentInvite("10000");

            Assert.AreEqual("10000", invite.TournamentInviteCode);
        }

        [TestMethod]
        public void Update_Tournament_Invite()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            TournamentInviteModel invite = db.GetTournamentInvite("10006");
            invite.TournamentInviteCode = "10055";
            var result = db.UpdateTournamentInvite(invite);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void DeleteTournamentInvite()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var result = db.DeleteTournamentInvite("10003");

            Assert.AreEqual(DbError.SUCCESS, result);
        }

    }
}
