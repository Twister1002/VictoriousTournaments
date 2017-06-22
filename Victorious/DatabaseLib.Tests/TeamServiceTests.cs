using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TeamServiceTests
    {
        IUnitOfWork unitOfWork;
        TeamService service;
        AccountService accountService;
        TournamentService tournamentService;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new TeamService(unitOfWork);
            accountService = new AccountService(unitOfWork);
            tournamentService = new TournamentService(unitOfWork);
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

            Assert.AreEqual(true, result);
        }

        #endregion

        #region TournamentTeamMembers

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTournamentTeamMember_Save()
        {
            TournamentTeamMemberModel tm = new TournamentTeamMemberModel()
            {
                SiteTeamMemberID = service.GetAllSiteTeamMembers()[0].SiteTeamMemberID,
                TournamentTeamID = service.GetAllTournamentTeams()[0].TournamentTeamID
            };
            service.AddTournamentTeamMember(tm);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTournamentTeamMember_Save()
        {
            var tm = service.GetTournamentTeamMember(1);

            Assert.AreEqual(2, tm.SiteTeamMemberID);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTournamentTeamMembers()
        {
            var tms = service.GetAllTournamentTeamMembers();

            Assert.AreEqual(1, tms.Count);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTournamentTeamMember_Save()
        {
            var tms = service.GetAllTournamentTeamMembers()[0];
            tms.SiteTeamMember = service.GetAllSiteTeams()[0].SiteTeamMembers.ToList()[1];
            service.UpdateTournamentTeamMember(tms);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTournamentTeamMemeber_Save()
        {
            service.DeleteTournamentTeamMember(service.GetAllTournamentTeams()[0].TournamentTeamMembers.ToList()[0].TournamentTeamMemberID);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion

        #region TournamentTeamBrackets

        [TestMethod]
        [TestCategory("Team Service")]
        public void AddTournamentTeamBracket_Save()
        {
            TournamentTeamBracketModel t = new TournamentTeamBracketModel()
            {
                BracketID = tournamentService.GetAllBrackets()[0].BracketID,
                TournamentTeamID = service.GetAllTournamentTeams()[0].TournamentTeamID,
                Seed = 1,
            };
            service.AddTournamentTeamBracket(t);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetTournamentTeamBracket()
        {
            var t = service.GetTournamentTeamBracket(2,1);

            Assert.AreEqual(1, t.Seed);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void GetAllTournamentTeamBrackets()
        {
            var ts = service.GetAllTournamentTeamBrackets();

            Assert.AreEqual(1, ts.Count);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void UpdateTournamentTeamBracket_Save()
        {
            var t = service.GetAllTournamentTeamBrackets()[0];
            t.Seed = 2;
            service.UpdateTournamentTeamBracket(t);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Team Service")]
        public void DeleteTournamentTeamBracket()
        {
            service.DeleteTournamentTeamBracket(service.GetAllTournamentTeamBrackets()[0]);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion

    }
}
