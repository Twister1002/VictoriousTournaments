using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class GameTypeTests
    {
        [TestMethod]
        public void Add_GameType()
        {
            var db = new DbInterface();

            GameTypeModel gameType = new GameTypeModel()
            {
                Title = "Borderlands",
            };
            var result = db.AddGameType(gameType);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_All_GameTypes()
        {
            var db = new DbInterface();

            List<GameTypeModel> games = db.GetAllGameTypes();

            Assert.AreEqual(2, games.Count);

        }


        [TestMethod]
        public void Update_GameTypes()
        {
            var db = new DbInterface();

            GameTypeModel game = db.GetAllGameTypes()[0];
            game.Title = "Rocket League";
            db.UpdateGameType(game);

            Assert.AreEqual("Rocket League", db.GetAllGameTypes()[0].Title);
        }

        [TestMethod]
        public void Delete_GameType()
        {
            var db = new DbInterface();

            GameTypeModel game = db.GetAllGameTypes()[0];
            db.DeleteGameType(game.GameTypeID);
            int result = db.GetAllGameTypes().Count;

            Assert.AreEqual(0, result);

        }
    }
}
