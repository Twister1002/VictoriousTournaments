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
            var db = new DatabaseRepository("VictoriousEntities");

            var result = db.GetAllBracketTypes().Count;

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void Update_Bracket_Types()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var bracketType = db.GetAllBracketTypes()[0];
            bracketType.IsActive = false;
            var result = db.UpdateBracketType(bracketType);
            Assert.AreEqual(DbError.SUCCESS, result);
        }

    }
}
