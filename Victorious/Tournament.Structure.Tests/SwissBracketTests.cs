using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class SwissBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		public void SwissCtor_Constructs()
		{
			IBracket b = new SwissBracket();

			Assert.IsInstanceOfType(b, typeof(SwissBracket));
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		public void SwissCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new SwissBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SwissCtor_ThrowsNull_WithNullParameter()
		{
			IBracket b = new SwissBracket(null, 1);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[ExpectedException(typeof(BracketException))]
		public void SwissCreateBracket_ThrowsException_WithNegativeGPM()
		{
			IBracket b = new SwissBracket(new List<IPlayer>(), -1);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_GeneratesFirstRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.AreEqual(pList.Count / 2, b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		public void SwissCreateBracket_GeneratesOnlyOneRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.AreEqual(1, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_UsesStandardSlideMatchups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			// Match(1) is Player[0] vs Player[4]
			Assert.AreEqual(b.Players[b.NumberOfPlayers() / 2].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_GivesFirstRoundByeToTopSeed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 3);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			// Match(1) is Player[second] vs Player[last]
			Assert.AreEqual(b.Players[1].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("Swiss CreateBracket")]
		[TestCategory("AddSwissRound")]
		public void SwissCreateBracket_ByePlayerGetsPointsForAWin()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 9; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			Assert.IsTrue(b.Rankings[0].MatchScore > 0);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_CreatesSecondRoundWhenFirstFinishes()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int firstRoundMatches = b.NumberOfMatches;
			for (int n = 1; n <= firstRoundMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			Assert.AreEqual(2 * firstRoundMatches, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_MatchesWinnersAgainstEachOther()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int firstRoundMatches = b.NumberOfMatches;
			for (int n = 1; n <= firstRoundMatches; ++n)
			{
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
			}

			Assert.IsTrue(b.GetMatch(1).Players[(int)(b.GetMatch(1).WinnerSlot)].Id == b.GetMatch(4).Players[(int)PlayerSlot.Defender].Id &&
				b.GetMatch(2).Players[(int)(b.GetMatch(2).WinnerSlot)].Id == b.GetMatch(4).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SwissBracket")]
		[TestCategory("AddSwissRound")]
		public void AddSwissRound_GivesSecondRoundByeToTopSeed_ButDoesntRepeatBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);
			int firstRoundMatches = b.NumberOfMatches;
			for (int n = 1; n <= firstRoundMatches; ++n)
			{
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
			}

			Assert.AreNotEqual(b.Players[0].Id, b.Rankings[0].Id);
		}

		[TestMethod]
		public void Swiss_RoundCountTester()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 13; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 5);
				pList.Add(moq.Object);
			}
			IBracket b = new SwissBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(1, 1);
		}
		#endregion
	}
}
