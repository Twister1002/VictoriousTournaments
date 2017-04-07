using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;
using DataLib;

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
            UserModel model = new UserModel()
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
            Assert.AreEqual<UserModel>(model, viewModel.Model);
        }
    }
}
