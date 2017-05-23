using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class RoundRobinBracketTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB Ctor")]
		public void RRBCtor_Constructs()
		{
			IBracket b = new RoundRobinBracket();

			Assert.IsInstanceOfType(b, typeof(RoundRobinBracket));
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB Ctor")]
		public void RRBCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new RoundRobinBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(3 * 2, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_AssignsCorrectMatchesPerPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int numMatchesForPlayerOne = 0;
			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch m = b.GetMatch(n);
				if (b.Players[0] == m.Players[(int)PlayerSlot.Defender]
					|| b.Players[0] == m.Players[(int)PlayerSlot.Challenger])
				{
					++numMatchesForPlayerOne;
				}
			}

			Assert.AreEqual(3, numMatchesForPlayerOne);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesFiveRounds_ForFivePlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(5, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesTenMatches_ForFivePlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(5 * 2, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_OverloadedCtorLimitsRoundCount()
		{
			int maxRounds = 4;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 1, maxRounds);

			Assert.AreEqual(maxRounds, b.NumberOfRounds);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddGame")]
		[TestCategory("Rankings")]
		public void RRBAddGame_UpdatesRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(1).Players[(int)(b.GetMatch(1).WinnerSlot)].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBAddWin_ThrowsInvalidIndex_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(-1, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBAddWin_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddGame(b.NumberOfMatches + 1, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB RemoveLastGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBRemoveLastGame_ThrowsInvalidIndex_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.RemoveLastGame(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB RemoveLastGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBRemoveLastGame_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.RemoveLastGame(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBResetScore_ThrowsInvalidIndex_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBResetScore_ThrowsNotFound_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("Rankings")]
		public void RRB_ScoreAndRankingTracker()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				IMatch m = b.GetMatch(n);
				if (m.Players[(int)PlayerSlot.Defender].Id > m.Players[(int)PlayerSlot.Challenger].Id)
				{
					b.AddGame(n, 1, 0, PlayerSlot.Defender);
				}
				else
				{
					b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				}
			}

			Assert.AreEqual(1, 1);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ReplacePlayer")]
		public void RRB_ReplacePlayer_Replaces()
		{
			Mock<IPlayer> playerMoq = new Mock<IPlayer>();
			playerMoq.Setup(p => p.Id).Returns(10);

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = 3;
			b.ReplacePlayer(playerMoq.Object, pIndex);
			Assert.AreEqual(playerMoq.Object, b.Players[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("ReplacePlayer")]
		public void RRB_ReplacePlayer_ReplacesPlayerInRankings()
		{
			Mock<IPlayer> playerMoq = new Mock<IPlayer>();
			playerMoq.Setup(p => p.Id).Returns(10);

			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList, 3);
			List<IMatch> firstRound = b.GetRound(1);
			foreach (IMatch match in firstRound)
			{
				b.AddGame(match.MatchNumber, 1, 0, PlayerSlot.Defender);
				b.AddGame(match.MatchNumber, 0, 1, PlayerSlot.Challenger);
				b.AddGame(match.MatchNumber, 1, 0, PlayerSlot.Defender);
			}

			int pIndex = 3;
			int rankIndex = b.Rankings.FindIndex(r => r.Id == b.Players[pIndex].Id);
			b.ReplacePlayer(playerMoq.Object, pIndex);
			Assert.AreEqual(playerMoq.Object.Id, b.Rankings[rankIndex].Id);
		}
		#endregion
	}
}
