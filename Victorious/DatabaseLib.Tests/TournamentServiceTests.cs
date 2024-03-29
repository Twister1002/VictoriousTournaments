﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseLib.Tests
{
    [TestClass]

    public class TournamentServiceTests
    {
        IUnitOfWork unitOfWork;
        TournamentService service;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork("StringTest");
            service = new TournamentService(unitOfWork);
        }

        #region Tournaments

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddTournament_Save()
        {
            TournamentModel tournament = NewTournament();
            tournament.Description = Guid.NewGuid().ToString();
            tournament.InviteCode = "1234";
            service.AddTournament(tournament);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetTournament()
        {
            var result = service.GetTournament(3);

            Assert.AreEqual(2, result.GameTypeID);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateTournament_Save()
        {
            var tournament = service.GetTournament(3);
            tournament.Title = "New Title";
            service.UpdateTournament(tournament);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteTournament_Save()
        {
            service.DeleteTournament(3);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void Search_By_Dates_And_Strings()
        {
           
            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("TournamentStartDate", DateTime.Today.ToString());
            dict.Add("Title", "Test Tournament One");
            dict.Add("GameTypeID", "1");
            dict.Add("InProgress", "False");
            var count = 1;
            tournaments = service.FindTournaments(dict, count);
            var result = tournaments.Count;

            Assert.AreEqual(count, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void Search_By_Dates_Between()
        {
            List<TournamentModel> tournaments = new List<TournamentModel>();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("TournamentStartDate", DateTime.Today.ToString());
            dict.Add("TournamentEndDate", DateTime.Today.AddDays(10).ToString());

            tournaments = service.FindTournaments(dict);
            var result = false;
            foreach (var tournament in tournaments)
            {
                if (tournament.Title == "Test Tournament Search")
                    result = true;
            }

            Assert.AreEqual(true, result);
        }




        private TournamentModel NewTournament()
        {
            TournamentModel tournament = new TournamentModel()
            {
                Title = "Test Tournament Search",
                Description = "Test",
                RegistrationStartDate = DateTime.Now,
                RegistrationEndDate = DateTime.Now.AddDays(2),
                TournamentStartDate = DateTime.Now,
                TournamentEndDate = DateTime.Now.AddDays(2),
                CheckInBegins = DateTime.Now,
                CheckInEnds = DateTime.Now,
                LastEditedByID = 1,
                CreatedByID = 1,
                PlatformID = 3,
                EntryFee = 0,
                PrizePurse = 0,
                GameTypeID = 1,
                PublicViewing = true,
                PublicRegistration = true
            };

            return tournament;
        }



        #endregion


        #region TournamentInvites

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddTournamentInvite_Save()
        {
            TournamentInviteModel invite = new TournamentInviteModel()
            {
                DateCreated = DateTime.Today,
                DateExpires = DateTime.Today.AddDays(1),
                IsExpired = false,
                TournamentInviteCode = "1235",
                NumberOfUses = 0,
                TournamentID = service.GetAllTournaments()[0].TournamentID
            };

            service.AddTournamentInvite(invite);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetTournamentInvite()
        {
            var invite = service.GetTournamentInvite("1234");
            var tournament = service.GetTournament(2);

            Assert.AreEqual(tournament, invite.Tournament);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateTournamentInvite_Save()
        {
            var expected = false;
            var invite = service.GetTournamentInvite("1234");
            invite.IsExpired = expected;
            service.UpdateTournamentInvite(invite);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteTournamentInvite_Save()
        {
            service.DeleteTournamentInvite("1234");
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void TournamentInviteExists()
        {
            
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetAllTournamentInvites()
        {

        }

        #endregion


        #region TournamentUsers

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddTournamentUser_Save()
        {
            TournamentUserModel tournamentUser = NewTournamentUser();
            tournamentUser.Name = unitOfWork.AccountRepo.GetAll().ToList()[0].Username;
            tournamentUser.TournamentID = service.GetAllTournaments()[0].TournamentID;
            service.AddTournamentUser(tournamentUser);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        private TournamentUserModel NewTournamentUser()
        {
            TournamentUserModel user = new TournamentUserModel()
            {
                AccountID = 1,
                Name = "Kelton",
                //Username = Guid.NewGuid().ToString(),
                UniformNumber = 1
            };
            return user;
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetTournamentUser()
        {
            var tournamentUser = service.GetTournamentUser(1);
            var result = tournamentUser.TournamentID;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateTournamentUser_Save()
        {
            var tournamentUser = service.GetTournamentUser(1);
            tournamentUser.Name = "New Name";
            service.UpdateTournamentUser(tournamentUser);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteTournamentUser_Save()
        {
            service.DeleteTournamentUser(1);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void CheckUserIn_Save()
        {
            service.CheckUserIn(1);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion


        #region Brackets

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddBracket_Save()
        {
            var bracket = new BracketModel()
            {
                //BracketTitle = "Bracket",
                BracketTypeID = 2,
                Finalized = true,
                TournamentID = service.GetAllTournaments()[0].TournamentID,
                NumberOfGroups = 2
            };
            service.AddBracket(bracket);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetBracket()
        {
            var bracket = service.GetBracket(1);

            Assert.AreEqual(1, bracket.BracketID);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateBracket_Save()
        {
            var bracket = service.GetBracket(1);
            bracket.Finalized = false;
            service.UpdateBracket(bracket);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteBracket()
        {
            service.DeleteBracket(1);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetAllBracketsInTournament()
        {
            List<BracketModel> brackets = new List<BracketModel>();
            brackets = service.GetAllBracketsInTournament(1);

            Assert.AreEqual(1, brackets.Count);
        }

        #endregion


        #region Matches

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddMatch_Save()
        {
            int tournamentId = service.GetAllTournaments()[0].TournamentID;
            MatchModel match = new MatchModel()
            {
                BracketID = service.GetAllBracketsInTournament(tournamentId)[0].BracketID,
                ChallengerID = service.GetAllUsersInTournament(tournamentId)[0].TournamentUserID,
                DefenderID = service.GetAllUsersInTournament(tournamentId)[1].TournamentUserID,
                MatchNumber = 1
            };
            service.AddMatch(match);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetMatch()
        {
            MatchModel match = service.GetMatch(1);
            MatchModel match2 = service.GetAllMatches()[0];

            Assert.AreEqual(match, match2);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateMatch_Save()
        {
            MatchModel match = service.GetAllMatches()[0];
            match.ChallengerScore = 10;
            service.UpdateMatch(match);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteMatch_Save()
        {
            service.DeleteMatch(1);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetAllMatchesInBracket()
        {
            var result = service.GetAllMatchesInBracket(1).Count;

            Assert.AreEqual(1, result);
        }

        #endregion


        #region Games

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddGame_Save()
        {
            GameModel game = new GameModel()
            {
                MatchID = service.GetAllMatches()[0].MatchID,
                ChallengerID = service.GetAllMatches()[0].ChallengerID,
                DefenderID = service.GetAllMatches()[0].DefenderID,
                GameNumber = 3
            };
            service.AddGame(game);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetGame()
        {
            GameModel game = service.GetGame(1);
            GameModel game2 = service.GetAllGames()[0];

            Assert.AreEqual(game, game2);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateGame_Save()
        {
            GameModel game = service.GetAllGames()[0];
            game.ChallengerScore = 10;
            service.UpdateGame(game);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteGame_Save()
        {
            service.DeleteGame(service.GetAllGames()[0].GameID);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }
    

        #endregion


        #region TournamentUsersBrackets

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddTournamentUserBracket_Save()
        {
            TournamentUsersBracketModel t = new TournamentUsersBracketModel()
            {
                TournamentUserID = service.GetAllTournamentUsers()[0].TournamentUserID,
                BracketID = service.GetAllBrackets()[0].BracketID,
                TournamentID = service.GetAllTournaments()[0].TournamentID,
                Seed = 1
            };
            service.AddTournamentUsersBracket(t);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void GetTournamentUserBracket()
        {
            var t = service.GetTournamentUsersBracket(2,2);

            Assert.AreEqual(3, t.Seed);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void UpdateTournamentUserBracket()
        {
            var t = service.GetTournamentUsersBracket(2, 2);
            t.Seed = 5;
            service.UpdateTournamentUsersBracket(t);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void DeleteTournamentUserBracket()
        {
            service.DeleteTournamentUsersBracket(2, 2);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion


       

    }

}
