using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	#region Bracket Creation
	[TestClass]
	public class SingleElimBracketTests
	{
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		public void SEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(pList, b.Players);
		}
#if false
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBCtor_ThrowsNullRef_WithNullParam()
		{
			IBracket b = new SingleElimBracket(null);

			Assert.AreEqual(1, 2);
		}
#endif
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB Ctor")]
		public void SEBCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new SingleElimBracket();
			//b.CreateBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(3, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_AssignsR1MatchNumbers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 10; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(9, b.GetRound(b.NumberOfRounds)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_DoesNotAssignToSecondRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_4Players_AssignsToRound1()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.Players[3],
				b.GetRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_5Players_1FirstRoundMatch_and_2SecondRoundMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyGeneratesMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(5 == b.NumberOfRounds
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(15 == b.GetRound(1)[0].Players[(int)PlayerSlot.Defender].Id
				&& 16 == b.GetRound(1)[0].Players[(int)PlayerSlot.Challenger].Id);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.IsNull(b.GrandFinal);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		public void SEBAddWin_4Players_UpdatesScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(1, b.GetRound(1)[0].Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		public void SEBAddWin_FinishedMatch_DoesNotSetBracketIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, PlayerSlot.Challenger);

			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		public void SEBAddWin_FinishedFinals_SetsBracketIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[TestCategory("Rankings")]
		public void SEBAddWin_FinishedMatch_AddsLoserToRankingsArray()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, PlayerSlot.Defender);
			
			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[TestCategory("Rankings")]
		public void SEBAddWin_FinishedBracket_AddsWinnerToTopOfRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(b.NumberOfMatches).Players[(int)PlayerSlot.Defender].Id);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
		[ExpectedException(typeof(InvalidSlotException))]
		public void SEBAddWin_ThrowsInvalidSlot_WithBadPlayerSlotParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(2, PlayerSlot.Challenger);

			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Challenger],
				b.GetRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBAddWin_ThrowsInvalidIndex_WithNegativeMatchNum()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(-1, 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddWin")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBAddWin_ThrowsNotFound_WhenMatchParamIsNotFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.NumberOfMatches + 1, 0);

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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetWinsNeeded(2);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetWinsNeeded(2);
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(1, PlayerSlot.Challenger);
			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(b.GetRound(1)[0].Players[(int)PlayerSlot.Defender],
				b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		public void SEBSubtractWin_ReversingFinalsVictory_SetsBracketIsNOTFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Challenger);
			}
			b.SubtractWin(b.NumberOfMatches, PlayerSlot.Challenger);

			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		public void SEBSubtractWin_DoesNotToggleIsFinished_WhenVictoryIsNotReversed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Challenger);
			}
			b.GetMatch(b.NumberOfMatches).SetWinsNeeded(2);
			b.AddWin(b.NumberOfMatches, PlayerSlot.Defender);
			b.AddWin(b.NumberOfMatches, PlayerSlot.Challenger);
			b.AddWin(b.NumberOfMatches, PlayerSlot.Defender);
			b.SubtractWin(b.NumberOfMatches, PlayerSlot.Challenger);
			// This leaves final match at 2-0 (still finished)

			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		[TestCategory("Rankings")]
		public void SEBSubtractWin_ReactivatedMatch_UpdatesRankingsArray()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBSubtractWin_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.SubtractWin(0, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB SubtractWin")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBSubtractWin_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetWinsNeeded(2);
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_UpdatesRankingsArray()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.ResetMatchScore(1);

			Assert.AreEqual(1, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBResetMatchScore_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBResetMatchScore_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_ResetsFutureScores()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.GetMatch(3).SetWinsNeeded(3);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Defender],
				b.GetMatch(3).Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEB_RPFFM_RemovesPlayerFromManyMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			for (int n = 1; n < 32; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.IsNull(b.GetMatch(31).Players[(int)PlayerSlot.Defender]);
		}
		#endregion

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void SEBNumberOfLowerRounds_Returns0()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(0, b.NumberOfLowerRounds);
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
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var x = b.GetLowerRound(1);

			Assert.AreEqual(1, 2);
		}
	}
}
