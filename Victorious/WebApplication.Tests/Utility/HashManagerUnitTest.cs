using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Utility;

namespace WebApplication.Tests.Utility
{
    [TestClass]
    public class HashManagerUnitTest
    {
        [TestMethod]
        [TestCategory("Hashing")]
        public void HashManager_GetSalt_Returns_Salt()
        {
            String salt = HashManager.GetSalt();

            Assert.IsNotNull(salt);
        }

        [TestMethod]
        [TestCategory("Hashing")]
        public void HashManager_HashPassword_HashesPassword()
        {
            String pass = "Test";
            String salt = HashManager.GetSalt();
            String hash = HashManager.HashPassword(pass, salt);

            Assert.IsNotNull(hash);
        }

        [TestMethod]
        [TestCategory("Hashing")]
        public void HashManager_ValidatePassword_Returns_True()
        {
            String pass = "Test";
            String salt = HashManager.GetSalt();
            String hash = HashManager.HashPassword(pass, salt);

            bool isPass = HashManager.ValidatePassword(pass, hash);

            Assert.AreEqual(true, isPass);
        }
    }
}
