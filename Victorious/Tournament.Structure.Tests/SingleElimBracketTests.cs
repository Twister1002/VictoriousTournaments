﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class SingleElimBracketTests
	{
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		public void SEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			pList.Add(new Mock<IPlayer>().Object);
			pList.Add(new Mock<IPlayer>().Object);
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(pList, b.Players);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBCtor_ThrowsNullRef_WithNullParam()
		{
			IBracket b = new SingleElimBracket(null);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SEBCreateBracket_ThrowsOutOfRange_WithLessThanTwoPlayers()
		{
			IBracket b = new SingleElimBracket();
			//b.CreateBracket();

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(2, b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsR1MatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(1, b.GetRound(1)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsR2MatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(3, b.GetRound(2)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsFinalRoundMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(9, b.GetRound(b.NumberOfRounds())[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_DoesNotAssignToSecondRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(-1, b.GetRound(2)[0].DefenderIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_AssignsToRound1()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(3, b.GetRound(1)[0].ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_5Players_1FirstRoundMatch_and_2SecondRoundMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue
				(b.GetRound(1).Count == 1
				&& b.GetRound(2).Count == 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_5Players_CorrectlyAssignsBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(-1, b.GetRound(2)[0].ChallengerIndex());
			// PlayerIndex==-1 is (no player), implying prev round's winner
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyGeneratesMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(5 == b.NumberOfRounds()
				&& 13 == b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyAssignsPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(15 == b.GetRound(1)[0].DefenderIndex()
				&& 16 == b.GetRound(1)[0].ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBCreateBracket_SetsLowerRoundsToNull()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			var x = b.GetLowerRound(1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_SetsGrandFinalToNull()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.IsNull(b.GetGrandFinal());
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		public void SEBAddWin_4Players_UpdatesScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(1, b.GetRound(1)[0].Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SEBAddWin_ThrowsInactive_WhenMatchIsAlreadyOver()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, PlayerSlot.Challenger); // ends match
			b.AddWin(1, PlayerSlot.Challenger); // throws exception

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SEBAddWin_ThrowsOutOfRange_WithBadPlayerSlotParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, (PlayerSlot)4);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		public void SEBAddWin_4Players_AdvancesWinningPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			int playerNum = b.GetRound(1)[1].ChallengerIndex();
			b.AddWin(2, PlayerSlot.Challenger);

			Assert.AreEqual(playerNum, b.GetRound(2)[0].ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void SEBAddWin_ThrowsKeyNotFound_WhenMatchParamIsNotFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(-1, 0);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		public void SEBSubtractWin_Subtracts()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).WinsNeeded = 2;
			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		public void SEBSubtractWin_DoesntIterateIfMatchDoesntReset()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).WinsNeeded = 2;
			int pIndex = b.GetMatch(1).DefenderIndex();
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(1, PlayerSlot.Challenger);
			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(pIndex, b.GetMatch(3).DefenderIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SEBSubtractWin_ThrowsOutOfRange_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SubtractWin(0, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void SEBSubtractWin_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SubtractWin(5, 0);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_Resets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).WinsNeeded = 2;
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SEBResetMatchScore_ThrowsOutOfRange_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void SEBResetMatchScore_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetMatchScore(6);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_RemovesPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(-1, b.GetRound(2)[0].DefenderIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_ResetsFutureScores()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.GetMatch(3).WinsNeeded = 3;
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Challenger);
			b.AddWin(3, PlayerSlot.Challenger);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(0, b.GetMatch(3).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_OnlyRemovesPlayerInQuestionFromFuture()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			int pIndex = b.GetMatch(2).DefenderIndex();
			b.AddWin(2, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(pIndex, b.GetMatch(3).ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_RemovesPlayerFromManyMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			int pIndex = b.GetMatch(1).DefenderIndex();
			for (int n = 1; n < 32; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(-1, b.GetMatch(31).DefenderIndex());
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void SEBNumberOfLowerRounds_Returns0()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(0, b.NumberOfLowerRounds());
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Accessors")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBGetLowerRound_ThrowsNullRef()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var x = b.GetLowerRound(1);

			Assert.AreEqual(1, 2);
		}
	}
}
