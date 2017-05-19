using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void Add_Game()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            GameModel game = new GameModel()
            {
                MatchID = 2,
                ChallengerID = db.GetMatch(2).ChallengerID,
                DefenderID = db.GetMatch(2).DefenderID,
                GameNumber = 1
            };

            var result = db.AddGame(game);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Game()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var result = db.GetGame(1);

            Assert.AreEqual(1, result.GameNumber);
        }

        [TestMethod]
        public void Update_Game_No_Cascade()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var game = db.GetGame(1);
            game.GameNumber = 2;
            var result = db.UpdateGame(game);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Delete_Game()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var result = db.DeleteGame(1);

            Assert.AreEqual(DbError.SUCCESS, result);
        }
    }
}
