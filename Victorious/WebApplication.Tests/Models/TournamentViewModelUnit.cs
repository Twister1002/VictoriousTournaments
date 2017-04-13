using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DataLib;
using WebApplication.Models;
using System.Linq;

namespace WebApplication.Tests.Models
{
    [TestClass]
    public class TournamentViewModelUnit
    {
        [TestMethod]
        [TestCategory("TournamentModel")]
        public void TournamentViewModel_SetModel_SetsValidModel()
        {
            // Arrange
            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel();

            // Act 
            viewModel.SetModel(model);

            // Assert
            Assert.AreEqual(model, viewModel.Model);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        public void TournamentViewModel_SetModel_SetsNullModel()
        {
            // Arrange
            TournamentViewModel viewModel = new TournamentViewModel();

            // Act 
            viewModel.SetModel(null);

            // Assert
            Assert.AreEqual(null, viewModel.Model);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        public void TournamentViewModel_Constructor_SetsModel()
        {
            // Arrange
            TournamentModel model = CreateModel();

            // Act 
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Assert
            Assert.AreEqual(model, viewModel.Model);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_UserPermission_Returns_PermissionTournamentAdministrator()
        {
            // Arrange
            int userId = 3;
            Permission expected = Permission.TOURNAMENT_ADMINISTRATOR;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            Permission userPermission = viewModel.UserPermission(userId);

            // Assert
            Assert.AreEqual(expected, Permission.TOURNAMENT_ADMINISTRATOR);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_UserPermission_Returns_PermissionTournamentStandard()
        {
            // Arrange
            int userId = 1;
            Permission permission = Permission.TOURNAMENT_STANDARD;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            Permission userPermission = viewModel.UserPermission(userId);

            // Assert
            Assert.AreEqual(permission, Permission.TOURNAMENT_STANDARD);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_UserPermission_Returns_PermissionNone()
        {
            // Arrange
            int userId = 7;
            Permission permission = Permission.TOURNAMENT_STANDARD;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            Permission userPermission = viewModel.UserPermission(userId);

            // Assert
            Assert.AreEqual(userPermission, Permission.NONE);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_Participants_Returns_4()
        {
            // Arrange
            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act

            // Assert
            Assert.AreEqual(4, viewModel.Participants.Count);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_Administrators_Returns_2()
        {
            // Arrange
            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act

            // Assert
            Assert.AreEqual(2, viewModel.Administrators.Count);
        }

        private TournamentModel CreateModel()
        {
            TournamentModel model = new TournamentModel()
            {
                TournamentID = 1,
                CreatedByID = 3,
                TournamentRulesID = 1,
                TournamentRules = new TournamentRuleModel()
                {
                    IsPublic = true,
                    RegistrationStartDate = DateTime.Now,
                    RegistrationEndDate = DateTime.Now.AddDays(1),
                    TournamentStartDate = DateTime.Now.AddDays(2),
                    TournamentEndDate = DateTime.Now.AddDays(3),
                },
            };
            AddUsersToModel(model);
            CreateBracket(model);
            CreateMatches(model.Brackets.ElementAt(0));

            return model;
        }

        private void AddUsersToModel(TournamentModel model)
        {
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                UserID = 1
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                UserID = 2
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_ADMINISTRATOR,
                TournamentID = 1,
                UserID = 3
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                UserID = 4
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_ADMINISTRATOR,
                TournamentID = 1,
                UserID = 5
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                Permission = Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                UserID = 6
            });
            model.UsersInTournament.Add(new UserInTournamentModel()
            {
                TournamentID = 1,
                UserID = 7
            });
        }

        private void CreateBracket(TournamentModel model)
        {
            model.Brackets.Add(new BracketModel()
            {
                BracketID = 1,
                BracketTypeID = 1,
                BracketType = new BracketTypeModel()
                {
                    Type = BracketTypeModel.BracketType.SINGLE,
                    BracketTypeID = 1,
                    TypeName = "Single Eleminiation"
                },
                BracketTitle = "Unit Test",
                Finalized = false,
                Tournament = model
            });
        }

        private void CreateMatches(BracketModel model)
        {
            model.Matches.Add(new MatchModel()
            {

            });
        }
    }
}
