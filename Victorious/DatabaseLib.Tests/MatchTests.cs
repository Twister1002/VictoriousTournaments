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
            int tournamentId = db.GetAllTournaments()[0].TournamentID;
            MatchModel match = new MatchModel()
            {
                BracketID = db.GetAllBracketsInTournament(tournamentId)[0].BracketID,
                ChallengerID = db.GetAllUsersInTournament(tournamentId)[0].TournamentUserID,
                DefenderID = db.GetAllUsersInTournament(tournamentId)[1].TournamentUserID,
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
        public void Get_Challenger()
        {
            var db = new DbInterface();

            TournamentUserModel user  = db.GetMatch(db.GetAllMatchesInBracket(1)[0].MatchID).Challenger;
            TournamentUserModel user2 = db.GetAllUsersInTournament(3)[0];

            Assert.AreEqual(user, user2);
        }

        [TestMethod]
        public void Add_Match_And_Get_Challenger()
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

            TournamentUserModel user = match.Challenger;

            Assert.AreEqual(db.GetTournamentUser(match.ChallengerID), user);
        }

        [TestMethod]
        public void Update_Challenger()
        {
            var db = new DbInterface();

            TournamentUserModel user  = db.GetMatch(db.GetAllMatchesInBracket(1)[0].MatchID).Challenger;
            MatchModel match = db.GetAllMatchesInBracket(db.GetAllBracketsInTournament(db.GetAllTournaments()[0].TournamentID)[0].BracketID)[0];
            match.Challenger.LastName = "bob";
            var db2 = new DbInterface();
            
            db2.UpdateTournamentUser(user);

            Assert.AreEqual("bob", user.LastName);
        }

        [TestMethod]
        public void Update_Challenger_Via_UpdateMatch()
        {
            var db = new DbInterface();

            MatchModel match = new MatchModel();
            match = db.GetAllMatchesInBracket(db.GetAllBracketsInTournament(db.GetAllTournaments()[0].TournamentID)[0].BracketID)[0];

            match.Challenger.FirstName = "Billy";
            db.UpdateMatch(match);

            Assert.AreEqual("Billy", db.GetMatch(match.MatchID).Challenger.FirstName);
            
        }

        [TestMethod]
        public void Change_Challenger_By_ChallengerID()
        {
            var db = new DbInterface();

            MatchModel match = db.GetAllMatchesInBracket(db.GetAllBracketsInTournament(db.GetAllTournaments()[0].TournamentID)[0].BracketID)[0];
            var db2 = new DbInterface();
            match.ChallengerID = db2.GetAllUsersInTournament(db2.GetAllTournaments()[0].TournamentID)[2].TournamentUserID;
            db2.UpdateMatch(match);
            TournamentUserModel user = db.GetAllUsersInTournament(db2.GetAllTournaments()[0].TournamentID)[2];
            MatchModel match2 = db.GetMatch(match.MatchID);

            Assert.AreEqual(match2.Challenger, user);
        }
    }
}
