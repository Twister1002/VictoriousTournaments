﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class RoundRobinBracketTests
	{
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
				pList.Add(new Mock<IPlayer>().Object);
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int numMatchesForPlayerOne = 0;
			foreach (IMatch m in b.Matches.Values)
			{
				if (0 == m.DefenderIndex() ||
					0 == m.ChallengerIndex())
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			Assert.AreEqual(4, (b as RoundRobinBracket).Scores.Length);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB CreateBracket")]
		public void RRBCreateBracket_CreatesFiveRounds_ForFivePlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
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
				pList.Add(new Mock<IPlayer>().Object);
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList, maxRounds);

			Assert.AreEqual(maxRounds, b.NumberOfRounds);
		}

		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_AddsAPoint()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = b.GetMatch(1).DefenderIndex();
			b.AddWin(1, PlayerSlot.Defender);

			Assert.AreEqual(1, (b as RoundRobinBracket).Scores[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_DoesntAddPointsWhenAddWinFails()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = b.GetMatch(1).DefenderIndex();
			b.AddWin(1, PlayerSlot.Defender);
			try
			{
				b.AddWin(1, PlayerSlot.Defender);
			}
			catch (InactiveMatchException)
			{ }

			Assert.AreEqual(1, (b as RoundRobinBracket).Scores[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		public void RRBAddWin_AccumulatesScoreWithMultipleWins()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = b.GetMatch(1).ChallengerIndex();
			b.AddWin(1, PlayerSlot.Challenger);
			for (int i = 2; i <= b.NumberOfMatches; ++i)
			{
				if (b.Matches[i].DefenderIndex() == pIndex)
				{
					b.AddWin(i, PlayerSlot.Defender);
					break;
				}
				else if (b.Matches[i].ChallengerIndex() == pIndex)
				{
					b.AddWin(i, PlayerSlot.Challenger);
					break;
				}
			}

			Assert.AreEqual(2, (b as RoundRobinBracket).Scores[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void RRBAddWin_ThrowsOutOfRange_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.AddWin(-1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB AddWin")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RRBAddWin_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = b.GetMatch(1).DefenderIndex();
			try
			{
				b.SubtractWin(1, PlayerSlot.Defender);
			}
			catch (ArgumentOutOfRangeException)
			{ }

			Assert.AreEqual(0, (b as RoundRobinBracket).Scores[pIndex]);
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			int pIndex = b.GetMatch(1).ChallengerIndex();
			b.AddWin(1, PlayerSlot.Challenger);
			b.SubtractWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(0, (b as RoundRobinBracket).Scores[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB SubtractWin")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void RRBSubtractWin_ThrowsOutOfRange_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.SubtractWin(-1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB SubtractWin")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RRBSubtractWin_ThrowsNotFound_WithBadMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
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
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.CreateBracket(2);
			int pIndex = b.GetMatch(1).ChallengerIndex();
			b.AddWin(1, PlayerSlot.Challenger);
			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual(0, (b as RoundRobinBracket).Scores[pIndex]);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void RRBResetScore_ThrowsOutOfRange_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinBracket")]
		[TestCategory("RRB ResetMatchScore")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RRBResetScore_ThrowsNotFound_WithBadInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new RoundRobinBracket(pList);

			b.ResetMatchScore(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}
	}
}