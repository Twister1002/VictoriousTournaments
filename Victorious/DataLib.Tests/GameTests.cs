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
        int numGames = 0;
        [TestInitialize]
        public void TestSetup()
        {
            db = new DatabaseInterface();
            bracket = db.GetBracketByID(1);
            match = new MatchModel();
            match = db.GetMatchById(1);

            numGames = 3;
        }

        [TestMethod]
        public void Add_NumGames_To_Match()
        {          
            var result = 0;
           
            for (int i = 0; i < numGames; i++)
            {
                var game = new GameModel()
                {
                    ChallengerID = match.ChallengerID,
                    DefenderID = match.DefenderID,
                    MatchID = match.MatchID,
                    GameNumber = i
                };
                if (i != 1)
                {
                    game.ChallengerScore = 5;
                    game.DefenderScore = 3;
                    game.WinnerID = game.ChallengerID;
                }
                else
                {
                    game.ChallengerScore = 3;
                    game.DefenderScore = 5;
                    game.WinnerID = game.DefenderID;
                }

                result += (int)db.AddGame(match, game);
            }

            Assert.AreEqual(numGames, result);
        }

        [TestMethod]
        public void Update_1_Game_In_Match()
        {

        }

        [TestMethod]
        public void Get_NumGames_In_Match()
        {
            var db = new DatabaseInterface();

            var result = db.GetAllGamesInMatch(match).Count;

            Assert.AreEqual(numGames, result);
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
