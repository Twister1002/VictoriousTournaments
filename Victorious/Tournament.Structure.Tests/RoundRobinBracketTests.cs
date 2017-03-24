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
		public void RRBCreateBracket_MakesAScoresArray()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(4, (b as RoundRobinBracket).Scores.Count);
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
			IBracket b = new RoundRobinBracket(pList, maxRounds);

			Assert.AreEqual(maxRounds, b.NumberOfRounds);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_AddsAPoint()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			
			b.AddWin(1, PlayerSlot.Defender);

			Assert.AreEqual((uint)1,
				(b as RoundRobinBracket).Scores[b.GetMatch(1).Players[(int)PlayerSlot.Defender].Id]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[TestCategory("Rankings")]
		public void RRBAddWin_UpdatesRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);

			Assert.AreEqual(b.GetMatch(1).Players[(int)(b.GetMatch(1).WinnerSlot)].Id,
				b.Rankings[0]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_DoesntAddPointsWhenAddWinFails()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			try
			{
				b.AddWin(1, PlayerSlot.Defender);
			}
			catch (InactiveMatchException)
			{ }

			Assert.AreEqual((uint)1,
				(b as RoundRobinBracket).Scores[b.GetMatch(1).Players[(int)PlayerSlot.Defender].Id]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_AccumulatesScoreWithMultipleWins()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pId = b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id;
			b.AddWin(1, PlayerSlot.Challenger);
			for (int i = 2; i <= b.NumberOfMatches; ++i)
			{
				if (b.GetMatch(i).Players[(int)PlayerSlot.Defender].Id == pId)
				{
					b.AddWin(i, PlayerSlot.Defender);
					break;
				}
				else if (b.GetMatch(i).Players[(int)PlayerSlot.Challenger].Id == pId)
				{
					b.AddWin(i, PlayerSlot.Challenger);
					break;
				}
			}

			Assert.AreEqual((uint)2, (b as RoundRobinBracket).Scores[pId]);
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

			b.AddWin(-1, 0);
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

			b.AddWin(b.NumberOfMatches + 1, 0);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB SubtractWin")]
		public void RRBSubtractWin_DoesNotSubtractUnderZero()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);
			
			try
			{
				b.SubtractWin(1, PlayerSlot.Defender);
			}
			catch (ScoreException)
			{ }

			Assert.AreEqual((uint)0,
				(b as RoundRobinBracket).Scores[b.GetMatch(1).Players[(int)PlayerSlot.Defender].Id]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[TestCategory("RRB SubtractWin")]
		public void RRBSubtractWin_SubtractsScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddWin(1, PlayerSlot.Challenger);
			b.SubtractWin(1, PlayerSlot.Challenger);

			Assert.AreEqual((uint)0,
				(b as RoundRobinBracket).Scores[b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB SubtractWin")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRBSubtractWin_ThrowsInvalidIndex_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.SubtractWin(-1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB SubtractWin")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRBSubtractWin_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.SubtractWin(b.NumberOfMatches + 1, 0);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		public void RRBResetScore_SubtractsPlayerScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.CreateBracket(2);
			b.AddWin(1, PlayerSlot.Challenger);
			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual((uint)0,
				(b as RoundRobinBracket).Scores[b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id]);
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
		[TestCategory("Scores")]
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
				b.AddWin(n,
					(m.Players[(int)PlayerSlot.Defender].Id > m.Players[(int)PlayerSlot.Challenger].Id)
					? PlayerSlot.Defender
					: PlayerSlot.Challenger);
			}

			Assert.AreEqual(1, 1);
		}
		#endregion
	}
}
