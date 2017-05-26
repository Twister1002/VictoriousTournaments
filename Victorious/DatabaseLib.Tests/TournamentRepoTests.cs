using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace DatabaseLib.Tests
{
    [TestClass]
    public class TournamentRepoTests
    {

        IUnitOfWork unitOfWork;
        TournamentModel tournament;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            tournament = NewTournament();
        }

        [TestMethod]
        [TestCategory("Unit Of Work")]
        public void Add_Tournament()
        {
            tournament.InviteCode = "555";
            unitOfWork.TournamentRepository.AddTournament(tournament);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Unit Of Work")]
        public void Get_Tournament()
        {
            unitOfWork.TournamentRepository.AddTournament(tournament);
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
