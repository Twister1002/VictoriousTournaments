using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DatabaseLib.Services;


namespace DatabaseLib.Tests
{
    [TestClass]
    public class TournamentRepoTests
    {

        IUnitOfWork unitOfWork;
        TournamentModel tournament;
        TournamentService service;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new TournamentService(unitOfWork);
            tournament = NewTournament();
        }

        [TestMethod]
        [TestCategory("Unit Of Work")]
        public void Add_Tournament()
        {
            tournament.InviteCode = "512";
            service.AddTournament(tournament);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Unit Of Work")]
        public void Get_Tournament()
        {
            unitOfWork.TournamentRepo.Add(tournament);
        }

        public void Update_Tournament()
        {

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
                //Platform = 0,
                EntryFee = 0,
                GameTypeID = 1,
                PlatformID = 1
                //InviteCode = Guid.NewGuid().ToString()
            };

            return tournament;
        }
    }
}
