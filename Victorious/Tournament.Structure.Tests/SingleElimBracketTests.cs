using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class SingleElimBracketTests
	{
		#region Bracket Creation
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
		public void SEBCreateBracket_4Players_DoesNotAssignPlayersToSecondRound()
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
		public void SEBCreateBracket_4Players_AssignsPlayersToRound1()
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
		public void SEBCreateBracket_SetsLowerRoundsToEmptyDict()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			List<IMatch> lowerRound1 = b.GetLowerRound(1);
			Assert.AreEqual(0, lowerRound1.Count);
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
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_4Players_UpdatesScore()
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

			b.AddGame(1, 0, 20);
			Assert.AreEqual(1, b.GetRound(1)[0].Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_FinishedMatch_DoesNotSetBracketIsFinished()
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

			b.AddGame(1, 1, 5);
			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_FinishedFinals_SetsBracketIsFinished()
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
				b.AddGame(n, 1, 0);
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[TestCategory("Rankings")]
		public void SEBAddGame_FinishedMatch_AddsLoserToRankingsArray()
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

			b.AddGame(1, 5, 3);
			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(1).Players[(int)PlayerSlot.Challenger].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[TestCategory("Rankings")]
		public void SEBAddGame_FinishedBracket_AddsWinnerToTopOfRankings()
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
				b.AddGame(n, 1, 0);
			}
			Assert.AreEqual(b.Rankings[0].Id,
				b.GetMatch(b.NumberOfMatches).Players[(int)PlayerSlot.Defender].Id);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SEBAddGame_ThrowsInactive_WhenMatchIsAlreadyOver()
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

			b.AddGame(1, 0, 1);
			b.AddGame(1, 0, 1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		public void SEBAddGame_4Players_AdvancesWinningPlayer()
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

			b.AddGame(2, 5, 89);
			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Challenger],
				b.GetRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBAddGame_ThrowsInvalidIndex_WithNegativeMatchNum()
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

			b.AddGame(-1, 1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB AddGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBAddGame_ThrowsNotFound_WhenMatchParamIsNotFound()
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

			b.AddGame((b.NumberOfMatches + 1), 2, 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_SubtractsScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetMaxGames(3);
			b.AddGame(1, 1, 0);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_RemovesGame()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetMaxGames(3);
			b.AddGame(1, 1, 0);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.GetMatch(1).Games.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		public void SEBRemoveLastGame_ReversingFinalsVictory_SetsBracketIsNOTFinished()
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
				b.AddGame(n, 0, 20);
			}
			b.RemoveLastGame(b.NumberOfMatches);
			Assert.IsFalse(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[TestCategory("Rankings")]
		public void SEBRemoveLastGame_ReactivatedMatch_RemovesLoserFromRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddGame(1, 1, 0);
			b.RemoveLastGame(1);
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void SEBRemoveLastGame_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.RemoveLastGame(0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB RemoveLastGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void SEBRemoveLastGame_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.RemoveLastGame(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_ResetsScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetMaxGames(3);
			b.AddGame(1, 1, 0);
			b.AddGame(1, 0, 1);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		public void SEBResetMatchScore_RemovesGames()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.GetMatch(1).SetMaxGames(3);
			b.AddGame(1, 1, 0);
			b.AddGame(1, 0, 1);
			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Games.Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SEB ResetMatchScore")]
		[TestCategory("SEB RemovePlayerFromFutureMatches")]
		public void SEBResetMatchScore_RemovesAffectedLosersFromRankingsList()
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
				b.AddGame(n, 1, 0);
			}
			b.ResetMatchScore(1);
			// Only remaining Finished match is n=2
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

			b.ResetMatchScore(b.NumberOfMatches + 1);
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

			b.AddGame(1, 1, 0);
			b.RemoveLastGame(1);
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

			b.AddGame(1, 1, 0);
			b.AddGame(2, 1, 0);
			b.GetMatch(3).SetMaxGames(5);
			b.AddGame(3, 1, 0);
			b.AddGame(3, 1, 0);
			b.AddGame(3, 0, 1);
			b.AddGame(3, 0, 1);
			b.RemoveLastGame(1);
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

			b.AddGame(1, 1, 0);
			b.AddGame(2, 1, 0);
			b.RemoveLastGame(1);
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
				b.AddGame(n, 1, 0);
			}
			b.RemoveLastGame(1);
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
		public void SEBGetLowerRound_ReturnsEmptyList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new SingleElimBracket(pList);
			List<IMatch> lowerRound1 = b.GetLowerRound(1);

			Assert.AreEqual(0, lowerRound1.Count);
		}
	}
}
