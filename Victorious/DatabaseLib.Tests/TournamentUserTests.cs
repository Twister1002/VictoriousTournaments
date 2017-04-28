using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TournamentUserTests
    {
        [TestMethod]
        public void Add_Tournament_User()
        {
            var db = new DbInterface();

            var user = NewTournamentUser();
            user.TournamentID = db.GetAllTournaments()[0].TournamentID;

            //user.TournamentID = tournament.TournamentID;
            //db.AddTournament(tournament);
            var result = db.AddTournamentUser(user);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Tournament_User_By_Id()
        {
            var db = new DbInterface();
            var user = NewTournamentUser();

            var result = db.GetTournamentUserById(user.TournamentUserID);

            Assert.AreEqual("Ryan", result.FirstName);
        }

        [TestMethod]
        public void Delete_Tournament_User()
        {
            var db = new DbInterface();

            var result = db.DeleteTournamentUser(0);

            //Assert.AreEqual(DbError.SUCCESS, result);
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
