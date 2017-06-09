using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Models;
using DatabaseLib;
using Moq;
using WebApplication.Utility;
using DatabaseLib.Services;
using WebApplication.Interfaces;

namespace WebApplication.Tests.Models
{
    [TestClass]
    public class AccountUnitTest
    {
        AccountModel testAccount;

        public AccountUnitTest()
        {
            testAccount = new AccountModel()
            {
                AccountID = 1,
                Username = "test",
                Password = "test",
                PermissionLevel = (int)Permission.SITE_ADMINISTRATOR,
            };
        }

         
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
            
        [TestMethod]
        public void Account_Constructs()
        {
            // Arrange
            Mock<IService> work = new Mock<IService>();
            work.Setup(x => x.Account).Returns(new AccountService(new UnitOfWork()));
            Account account = new Account(work.Object, -1);

            // Act


            //Assert
            Assert.AreEqual<bool>(true, account.Model != null);
        }

        [TestMethod]
        public void Account_Constructor_LoadsModel()
        {
            // Arrange
            Mock<IService> work = new Mock<IService>();
            work.Setup(x => x.Account).Returns(new AccountService(new UnitOfWork()));
            work.Setup(x => x.Account.GetAccount(It.IsAny<int>())).Returns(testAccount);

            // Act
            Account account = new Account(work.Object, 1);

            //Assert
            Assert.AreEqual(testAccount, account.Model);
        }
    }
}
