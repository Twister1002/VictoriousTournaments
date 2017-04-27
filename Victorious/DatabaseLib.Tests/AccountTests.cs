using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class AccountTests
    {
        [TestMethod]
        public void Add_User_Acccount()
        {
            var db = new DbInterface();
            var user = new Account()
            {
                FirstName = "Ryan",
                LastName = "Kelton",
                Username = Guid.NewGuid().ToString(),
                Password = "1234",
                MyProperty = 0,
                PermissionLevel = (int)Permission.SITE_STANDARD,

               
            };

            var result = db.AddAccount(user);
            Assert.AreEqual(DbError.SUCCESS, result);
        }
    }
}
