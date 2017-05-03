using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class MatchTests
    {
        [TestMethod]
        public void Add_Match()
        {
            var db = new DbInterface();

            MatchModel match = new MatchModel()
            {
                BracketID = db.GetAllBracketsInTournament(db.GetAllTournaments()[0].TournamentID)[0].BracketID,
                ChallengerID = 1,
                DefenderID = 2,
                MatchNumber = 1
            };

            var result = db.AddMatch(match);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Match()
        {
            var db = new DbInterface();

            MatchModel match = db.GetMatch(1);

            Assert.AreEqual(3, match.BracketID);
        }

        [TestMethod]
        public void Update_Match_No_Cascade()
        {
            var db = new DbInterface();

            MatchModel match = db.GetMatch(1);

            match.ChallengerID = 7;

            var result = db.UpdateMatch(match);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Delete_Match()
        {
            var db = new DbInterface();

            var result = db.DeleteMatch(1);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Set_Challenger()
        {
            var db = new DbInterface();

            MatchModel match = new MatchModel()
            {
                BracketID = 3,
                ChallengerID = 1,
                DefenderID = 2,
                MatchNumber = 1
            };

            db.AddMatch(match);

            Assert.AreEqual(1, match.Challenger.TournamentUserID);

        }

        [TestMethod]
        public void Update_Challenger_Via_Update_Match()
        {
            var db = new DbInterface();

            MatchModel match = new MatchModel();
            match = db.GetAllMatchesInBracket(db.GetAllBracketsInTournament(db.GetAllTournaments()[0].TournamentID)[0].BracketID)[0];

            match.Challenger.FirstName = "Billy";
            db.UpdateMatch(match);

            Assert.AreEqual("Billy", db.GetMatch(match.MatchID).Challenger.FirstName);
            
        }
    }
}
