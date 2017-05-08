using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class PlatformTests
    {
        [TestMethod]
        public void Get_All_Platforms()
        {
            var db = new DbInterface();

            List<PlatformModel> platforms = new List<PlatformModel>();
            platforms = db.GetAllPlatforms();
            var result = platforms.Count;

            Assert.AreEqual(15, result);
        }
    }
}
