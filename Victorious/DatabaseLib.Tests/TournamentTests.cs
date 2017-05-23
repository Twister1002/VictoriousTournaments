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
            var db = new DatabaseRepository("VictoriousEntities");
            //var db = new DatabaseRepository();


            var tournament = NewTournament();
            tournament.InviteCode = "10009";
            var result = db.AddTournament(tournament);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Tournament()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var tournament = db.GetTournament(4);

            Assert.AreEqual("Test", tournament.Description);
        }

        [TestMethod]
        public void Update_Tournament_No_Cascade()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            //var tournament = NewTournament();
            var tournament = db.GetAllTournaments()[0];
            db.AddTournament(tournament);
            tournament.Description = "Test Me";
            tournament.InviteCode = "10007";
            db.UpdateTournament(tournament);
            var t = db.GetTournament(tournament.TournamentID);

            Assert.AreEqual("Test Me", t.Description);
        }

        [TestMethod]
        public void Update_Tournament_Cascade()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var tournament = db.GetTournament(4);
            var brackets = db.GetAllBracketsInTournament(tournament.TournamentID);
            brackets[0].Finalized = false;
            GameModel game = db.GetGame(3);
            var matches = db.GetAllMatchesInBracket(brackets[0].BracketID);
            matches[0].ChallengerID = 3;

            var result = db.UpdateTournament(tournament, true);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        //[TestMethod]
        //public void Update_Tournament_And_Invite_Code()
        //{
        //    var db = new DbInterface();

        //    var tournament = db.GetTournament(1004);
        //    tournament.InviteCode = "10002";
        //    var result = db.UpdateTournament(tournament);

        //    Assert.AreEqual(DbError.SUCCESS, result);

        //}

        [TestMethod]
        public void Delete_Tournaent()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var tournament = db.GetAllTournaments()[0];

            var result = db.DeleteTournament(tournament.TournamentID);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        public void Search_By_Title()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("Title", "Test Tournament One");
            tournaments = db.FindTournaments(dict);
            var result = tournaments.Count;

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Search_By_Start_Date()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("TournamentStartDate", DateTime.Today.ToShortDateString());
            tournaments = db.FindTournaments(dict);
            var result = tournaments.Count;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Search_Return_Default()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            var date = new DateTime();
            date = DateTime.Today.AddDays(1);
            dict.Add("TournamentStartDate", DateTime.Today.ToShortDateString());
            tournaments = db.FindTournaments(dict);
            var result = tournaments.Count;

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void Search_By_Dates_And_Strings()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("TournamentStartDate", DateTime.Today.ToString());
            dict.Add("Title", "test tournament one");
            dict.Add("GameTypeID", "1");
            dict.Add("InProgress", "false");
            var count = 3;
            tournaments = db.FindTournaments(dict, count);
            var result = tournaments.Count;

            Assert.AreEqual(count, result);
        }

        //[TestMethod]
        //public void Add_Invite_Code_GUID()
        //{
        //    var db = new DbInterface();

        //    var guid = Guid.NewGuid().ToString();
        //    var result = db.AddTournamentInviteCode(guid);

        //    Assert.AreEqual(DbError.SUCCESS, result);

        //}

        //[TestMethod]
        //public void Check_Invite_Code_Exists()
        //{
        //    var db = new DbInterface();

        //    var guid = "98ed7a71-b0f7-4c97-907a-e7b238b4daed";
        //    var result = db.InviteCodeExists(guid);

        //    Assert.AreEqual(DbError.EXISTS, result);

        //}



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
                PlatformID = 3,
                EntryFee = 0,
                PrizePurse = 0,
                GameTypeID = 1
            };

            return tournament;
        }

    }
}
