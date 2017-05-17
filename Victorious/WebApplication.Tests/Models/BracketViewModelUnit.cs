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
        [TestCategory("BracketViewModel")]
        [TestCategory("Bracket")]
        public void BracketViewModel_ResetMatches_FromMatch2_Returns_CorrectMatchesAffected()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(11)));
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

            // Assert

            Assert.AreEqual(true, (!matchesAffectedActual.Except(matchesAffected).Any() && !matchesAffected.Except(matchesAffectedActual).Any()));
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        public void BracketViewModel_IsPowerOfTwo_Value8_ReturnsTrue()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act
            bool result = model.IsPowerOfTwo(8);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        public void BracketViewModel_IsPowerOfTwo_Value64_ReturnsTrue()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act
            bool result = model.IsPowerOfTwo(64);

            //Assert
            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        public void BracketViewModel_IsPowerOfTwo_Value3_ReturnsFalse()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act
            bool result = model.IsPowerOfTwo(3);

            //Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        public void BracketViewModel_IsPowerOfTwo_Value0_ReturnsFalse()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel();

            // Act
            bool result = model.IsPowerOfTwo(0);

            //Assert
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("UpperBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_8Players_UpperBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(8)));
            List<bool> expected = new List<bool>() { true, true, false, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.UPPER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("UpperBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_3Players_UpperBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(3)));
            List<bool> expected = new List<bool>() { true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.UPPER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("UpperBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_5Players_UpperBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(5)));
            List<bool> expected = new List<bool>() { true, true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.UPPER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("UpperBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_11Players_UpperBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(11)));
            List<bool> expected = new List<bool>() { true, true, true, false, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.UPPER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("UpperBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_16Players_UpperBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(16)));
            List<bool> expected = new List<bool>() { true, true, false, true, false, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.UPPER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("LowerBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_8Players_LowerBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(8)));
            List<bool> expected = new List<bool>() { true, true, true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.LOWER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("LowerBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_3Players_LowerBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(3)));
            List<bool> expected = new List<bool>() { };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.LOWER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("LowerBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_5Players_LowerBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(5)));
            List<bool> expected = new List<bool>() { false, true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.LOWER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("LowerBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_11Players_LowerBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(11)));
            List<bool> expected = new List<bool>() { false, true, true, true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.LOWER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("BracketViewModel")]
        [TestCategory("LowerBracket")]
        public void BracketViewModel_DoubleElim_RoundShowing_16Players_LowerBracket()
        {
            // Arrange
            BracketViewModel model = new BracketViewModel(DoubleEliminationBracket(CreatePlayers(16)));
            List<bool> expected = new List<bool>() { true, true, true, true, true, true };

            // Act
            List<bool> result = model.RoundShowing(BracketSection.LOWER);

            //Assert
            CollectionAssert.AreEqual(expected, result);
        }

        // Helper Objects
        IBracket bracket;

        // Helper Methods
        private void ProcessMatches_DoubleElim()
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

        private List<IPlayer> CreatePlayers(int playersAmt)
        {
            List<IPlayer> players = new List<IPlayer>();

            for (int i = 1; i <= playersAmt; i++)
            {
                players.Add(new User(i, "Player "+i, "Player", i.ToString(), "Player"+i+"@email.com"));
            }

            return players;
        }

        private IBracket SingleEliminationBracket(List<IPlayer> players)
        {
            bracket = new SingleElimBracket(players);

            return bracket;
        }

        private IBracket DoubleEliminationBracket(List<IPlayer> players)
        {
            bracket = new DoubleElimBracket(players);

            return bracket;
        }
    }
}
