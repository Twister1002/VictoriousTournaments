using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLib.Tests
{
    [TestClass]
    public class GameTests
    {
        DatabaseInterface db = null;
        BracketModel bracket = null;
        MatchModel match = null;

        [TestInitialize]
        public void TestSetup()
        {
            db = new DatabaseInterface();
            bracket = db.GetBracketByID(1);
            match = new MatchModel();
            match = db.GetMatchById(1);
        }

        [TestMethod]
        public void Get_All_Games_In_Match()
        {
            var db = new DatabaseInterface();

            var result = db.GetAllGamesInMatch(match).Count;

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Delete_Game_Returns_Success()
        {
            var db = new DatabaseInterface();
            var game = db.GetAllGamesInMatch(match)[0];
            var result = db.DeleteGame(game);

            Assert.AreEqual(DbError.SUCCESS, result);

        }

    }
}
