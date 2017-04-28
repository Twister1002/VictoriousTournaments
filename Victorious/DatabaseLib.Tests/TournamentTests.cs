using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

            var tournament = db.GetTournamentById(1);

            Assert.AreEqual("Test", tournament.Description);
        }

        [TestMethod]
        public void Update_Tournament()
        {
            var db = new DbInterface();

            var tournament = NewTournament();
            db.AddTournament(tournament);
            tournament.Description = "Test Me";
            db.UpdateTournament(tournament);
            var t = db.GetTournamentById(tournament.TournamentID);

            Assert.AreEqual("Test Me", t.Description);
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
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = Guid.NewGuid().ToString(),
                Seed = 1,
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
                GameTypeID = 0,
                PrizePurse = 0,
            };

            return tournament;
        }

    }
}
