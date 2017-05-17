using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class BracketTypeTests
    {
        [TestMethod]
        public void Get_All_BracketTypes()
        {
            var db = new DatabaseRepository();

            var result = db.GetAllBracketTypes().Count;

            Assert.AreEqual(5, result);
        }
    }
}
