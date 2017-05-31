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
            tournament.InviteCode = Guid.NewGuid().ToString();
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

        




    }

}
