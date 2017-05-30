using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        IUnitOfWork unitOfWork;
        AccountService service;
        
        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new AccountService(unitOfWork);
        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void Add_User_Account()
        {
            var user = new AccountModel()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                //Username = Guid.NewGuid().ToString(),
                Username = "keltonr01",
                Email = "keltonr01@gmail.com",
                Password = "1234",
                PermissionLevel = (int)Permission.SITE_STANDARD
            };

            service.AddAccount(user);

            var result = unitOfWork.Save();
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void Account_UsernameExists_Exists()
        {

        }

        [TestMethod]
        [TestCategory("Account Service")]
        public void Get_Account()
        {

        }
    }
}
