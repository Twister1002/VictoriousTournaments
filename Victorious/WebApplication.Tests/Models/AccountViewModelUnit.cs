using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;
using DatabaseLib;

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
    }
}
