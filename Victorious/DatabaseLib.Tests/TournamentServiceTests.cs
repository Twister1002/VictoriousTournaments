using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;

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
            unitOfWork = new UnitOfWork();
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
                TournamentInviteCode = "1234",
                NumberOfUses = 0,
                TournamentID = 2
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

        #endregion


        #region TournamentUsers

        [TestMethod]
        [TestCategory("Tournament Service")]
        public void AddTournamentUser()
        {
            TournamentUserModel tournamentUser = NewTournamentUser();
            tournamentUser.TournamentID = service.GetAllTournaments()[0].TournamentID;
            service.AddTournamentUser(tournamentUser);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        private TournamentUserModel NewTournamentUser()
        {
            TournamentUserModel user = new TournamentUserModel()
            {

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
