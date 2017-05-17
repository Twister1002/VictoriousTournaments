using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using DatabaseLib;
using WebApplication.Models;
using System.Linq;

namespace WebApplication.Tests.Models
{
    [TestClass]
    public class TournamentViewModelUnit
    {
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
        public void TournamentViewModel_UserPermission_UserIsAdministrator()
        {
            // Arrange
            int userId = 3;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            bool isAdmin = viewModel.IsAdministrator(userId);

            // Assert
            Assert.AreEqual(true, isAdmin);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_UserPermission_UserIsNotAdministrator()
        {
            // Arrange
            int userId = 2;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            bool isAdmin = viewModel.IsAdministrator(userId);

            // Assert
            Assert.AreEqual(false, isAdmin);
        }

        [TestMethod]
        [TestCategory("TournamentModel")]
        [TestCategory("Permissions")]
        public void TournamentViewModel_UserPermission_UserHasNoPermissions()
        {
            // Arrange
            int userId = 7;

            TournamentModel model = CreateModel();
            TournamentViewModel viewModel = new TournamentViewModel(model);

            // Act 
            bool isAdmin = viewModel.IsAdministrator(userId);

            // Assert
            Assert.AreEqual(true, isAdmin);
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
            };
            AddUsersToModel(model);
            CreateBracket(model);
            CreateMatches(model.Brackets.ElementAt(0));

            return model;
        }

        private void AddUsersToModel(TournamentModel model)
        {
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                AccountID = 1
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                AccountID = 2
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_ADMINISTRATOR,
                TournamentID = 1,
                AccountID = 3
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                AccountID = 4
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_ADMINISTRATOR,
                TournamentID = 1,
                AccountID = 5
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                PermissionLevel = (int)Permission.TOURNAMENT_STANDARD,
                TournamentID = 1,
                AccountID = 6
            });
            model.TournamentUsers.Add(new TournamentUserModel()
            {
                TournamentID = 1,
                AccountID = 7
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
                    Type = BracketType.SINGLE,
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
