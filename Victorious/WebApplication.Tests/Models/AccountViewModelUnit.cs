using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;
using DatabaseLib;
using Moq;

namespace WebApplication.Tests.Models
{
    [TestClass]
    public class AccountViewModelUnit
    {
        [TestMethod]
        [TestCategory("AccountViewModel")]
        [TestCategory("ViewModels")]
        public void Account_Fields_ApplyFields_SetsFieldsToModel()
        {
            // Arrange
            Mock<DatabaseRepository> db = new Mock<DatabaseRepository>();
            db.Setup(x => x.AddAccount(GenerateModel()));

            AccountViewModel viewModel = new AccountViewModel();
            AccountModel model = new AccountModel()
            {
                FirstName = "Tyler",
                LastName = "Yeary",
                Username = "Twister",
                Email = "test@test.test",
                Password = "test"
            };

            viewModel.FirstName = "Tyler";
            viewModel.LastName = "Yeary";
            viewModel.Username = "Twister";
            viewModel.Email = "test@test.test";
            viewModel.Password = "test";

            // Act
            viewModel.ApplyChanges();

            // Assert
            Assert.AreEqual<AccountModel>(model, viewModel.Account);
        }



        private AccountModel GenerateModel()
        {
            AccountModel model = new AccountModel()
            {
                CreatedOn = DateTime.Now,
                Email = "UnitTesting@email.com",
                FirstName = "Site",
                LastName = "Admin",
                Password = "123",
                PermissionLevel = (int)Permission.SITE_ADMINISTRATOR,
                Username = "SiteAdmin"
            };

            return model;
        }
    }
}
