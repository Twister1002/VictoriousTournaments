using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TeamServiceTests
    {
        IUnitOfWork unitOfWork;
        TeamService service;
        AccountService accountService;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new TeamService(unitOfWork);
            accountService = new AccountService(unitOfWork);
        }

        #region SiteTeams

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTeam_Save()
        {
            SiteTeamModel team = new SiteTeamModel()
            {
                TeamName = "Team 2",
                DateCreated = DateTime.Now
            };
            service.AddSiteTeam(team);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTeam()
        {
            SiteTeamModel team = service.GetSiteTeam(4);

            Assert.AreEqual("Team 1", team.TeamName);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTeams()
        {
            List<SiteTeamModel> teams = service.GetAllSiteTeams();

            Assert.AreEqual(2, teams.Count);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTeam_Save()
        {
            var team = service.GetAllSiteTeams()[0];
            team.TeamName = "Team 1 Updated";
            service.UpdateSiteTeam(team);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTeam_Save()
        {
            service.DeleteSiteTeam(service.GetAllSiteTeams()[0].SiteTeamID);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion

        #region SiteTeamMembers

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddSiteTeamMember_Save()
        {
            SiteTeamMemberModel tm = new SiteTeamMemberModel()
            {
                Account = accountService.GetAllAccounts()[0],
                Role = Permission.TEAM_CREATOR,
                SiteTeamID = service.GetAllSiteTeams()[0].SiteTeamID
            };
            service.AddSiteTeamMemeber(tm);
            service.AddSiteTeamMemeber(tm);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetSiteTeamMember()
        {
            SiteTeamMemberModel tm = service.GetSiteTeamMemeber(1);

            Assert.AreEqual(5, tm.SiteTeamID);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllSiteTeamMembers()
        {
            var tms = service.GetAllSiteTeamMembers();

            Assert.AreEqual(1, tms.Count);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateSiteTeamMember_Save()
        {
            var tm = service.GetAllSiteTeamMembers()[0];
            tm.Role = Permission.TEAM_STANDARD;
            service.UpdateSiteTeamMember(tm);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteSiteTeamMember_Save()
        {
            service.DeleteSiteTeamMember(service.GetAllSiteTeamMembers()[0].SiteTeamMemberID);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion

        #region TournamentTeams

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTournamentTeam_Save()
        {
            TournamentTeamModel team = new TournamentTeamModel()
            {
                TeamName = "Team 1",
                TournamentID = 1,
                SiteTeam = service.GetAllSiteTeams()[0]                
            };
            service.AddTournamentTeam(team);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTournamentTeam()
        {
            var team = service.GetTournamentTeam(1);

            Assert.AreEqual("Team 1", team.TeamName);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTournamentTeams()
        {
            var teams = service.GetAllTournamentTeams();

            Assert.AreEqual(1, teams.Count);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTournamentTeam_Save()
        {
            var team = service.GetAllTournamentTeams()[0];
            team.TeamName = "Team 1 Updated";
            service.UpdateTournamentTeam(team);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTournamentTeam_Save()
        {
            service.DeleteTournamentTeam(service.GetAllTournamentTeams()[0].TournamentTeamID);
            var result = unitOfWork.Save();

            //Assert.AreEqual();
        }

        #endregion

        #region TournamentTeamMembers

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTournamentTeamMember_Save()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTournamentTeamMember_Save()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTournamentTeamMembers()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTournamentTeamMember_Save()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTournamentTeamMemeber_Save()
        {

        }

        #endregion

        #region TournamentTeamBrackets

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTournamentTeamBracket_Save()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTournamentTeamBracket()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTournamentTeamBrackets()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTournamentTeamBracket_Save()
        {

        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTournamentTeamBracket()
        {

        }

        #endregion

    }
}
