using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Models;
using Tournament.Structure;

namespace WebApplication.Tests.Models
{
    [TestClass]
    public class BracketViewModelUnit
    {
        [TestMethod]
        [TestCategory("BracketView")]
        public void BracketViewModel_GetRounds_Returns_NumberOfRounds()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act


            // Assert
        }

        [TestMethod]
        [TestCategory("BracketView")]
        [TestCategory("Bracket")]
        public void BracketViewModel_ResetMatches_FromMatch2_Returns_CorrectMatchesAffected()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers()));
            List<int> matchesAffectedActual = new List<int>();
            // Player 11
            matchesAffectedActual.Add(2);
            matchesAffectedActual.Add(6);
            matchesAffectedActual.Add(12);

            //Player 1
            matchesAffectedActual.Add(6);
            matchesAffectedActual.Add(9);
            matchesAffectedActual.Add(13);
            matchesAffectedActual.Add(15);

            //Player 3
            matchesAffectedActual.Add(12);
            matchesAffectedActual.Add(14);
            matchesAffectedActual.Add(15);
            matchesAffectedActual.Add(16);

            //Player 5
            matchesAffectedActual.Add(9);
            matchesAffectedActual.Add(10);
            matchesAffectedActual.Add(16);
            matchesAffectedActual.Add(17);

            // Player 2
            matchesAffectedActual.Add(17);
            
            matchesAffectedActual = matchesAffectedActual.Distinct().ToList();
            matchesAffectedActual.Sort();

            // Act
            List<int> matchesAffected = model.MatchesAffectedList(2);
            // Should be matches (2, 6, 12, 9, 13, 15, 10, 16, 17, 13, 15)


            // Assert
            Assert.AreEqual(true, !matchesAffectedActual.Except(matchesAffected).Any());
        }


        // Helper Objects
        IBracket bracket;

        // Helper Methods
        public void ProcessMatches_DoubleElim()
        {
            // Upper Bracket
            bracket.AddGame(1, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(2, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(3, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(4, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(5, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(6, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(7, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(8, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(9, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(10, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(11, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(12, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(13, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(14, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(15, 0, 1, PlayerSlot.Challenger);
            bracket.AddGame(16, 1, 0, PlayerSlot.Defender);
            bracket.AddGame(17, 1, 0, PlayerSlot.Defender);
        }

        public List<IPlayer> CreatePlayers()
        {
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 1; i <= 11; i++)
            {
                players.Add(new User(i, "Player "+i, "Player", i.ToString(), "Player"+i+"@email.com"));
            }

            return players;
        }

        public IBracket SingleEliminationBracket(List<IPlayer> players)
        {
            bracket = new SingleElimBracket(players);

            return bracket;
        }

        public IBracket DoubleEliminationBracket(List<IPlayer> players)
        {
            bracket = new DoubleElimBracket(players);

            return bracket;
        }
    }
}
