using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TournamentTests
    {

        [TestMethod]
        public void Add_Tournament()
        {
            var db = new DbInterface();

            var tournament = NewTournament();

            var result = db.AddTournament(tournament);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Tournament()
        {
            var db = new DbInterface();

            var tournament = db.GetTournament(1);

            Assert.AreEqual("Test", tournament.Description);
        }

        [TestMethod]
        public void Update_Tournament_No_Cascade()
        {
            var db = new DbInterface();

            var tournament = NewTournament();
            db.AddTournament(tournament);
            tournament.Description = "Test Me";
            db.UpdateTournament(tournament);
            var t = db.GetTournament(tournament.TournamentID);

            Assert.AreEqual("Test Me", t.Description);
        }

        [TestMethod]
        public void Update_Tournament_Cascade()
        {
            var db = new DbInterface();

            var tournament = db.GetAllTournaments()[0];
            var brackets = db.GetAllBracketsInTournament(tournament.TournamentID);
            brackets[0].Finalized = false;
            GameModel game = db.GetGame(3);
            var matches = db.GetAllMatchesInBracket(brackets[0].BracketID);
            matches[0].ChallengerID = 7;
            
            var result = db.UpdateTournament(tournament, true);

            Assert.AreEqual(DbError.SUCCESS, result);
        }



        [TestMethod]
        public void Delete_Tournaent()
        {
            var db = new DbInterface();

            var tournament = db.GetAllTournaments()[0];

            var result = db.DeleteTournament(tournament.TournamentID);

            Assert.AreEqual(DbError.SUCCESS, result);
        }
      

        private TournamentUserModel NewTournamentUser()
        {
            TournamentUserModel user = new TournamentUserModel()
            {
                Name = "Ryan",
                //Username = Guid.NewGuid().ToString(),
                UniformNumber = 1
            };
            return user;
        }


        private TournamentModel NewTournament()
        {
            TournamentModel tournament = new TournamentModel()
            {
                Title = "Test Tournament One",
                Description = "Test",
                RegistrationStartDate = DateTime.Now,
                RegistrationEndDate = DateTime.Now,
                TournamentStartDate = DateTime.Now,
                TournamentEndDate = DateTime.Now,
                CheckInBegins = DateTime.Now,
                CheckInEnds = DateTime.Now,
                LastEditedByID = 1,
                CreatedByID = 1,
                Platform = 0,
                EntryFee = 0,
                PrizePurse = 0,
            };

            return tournament;
        }

    }
}
