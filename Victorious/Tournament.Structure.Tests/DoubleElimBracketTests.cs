using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class DoubleElimBracketTests
	{
#region Bracket Creation
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB Ctor")]
		public void DEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			Assert.IsInstanceOfType(b, typeof(DoubleElimBracket));
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB Ctor")]
		public void DEBCtor_CallsSEBCtor()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			Assert.AreEqual(2, b.Players.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB Ctor")]
		public void DEBCtor_CreatesNoMatches_WithLessThanTwoPlayers()
		{
			IBracket b = new DoubleElimBracket();

			Assert.AreEqual(0, b.NumberOfMatches);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_InheritsUpperBracketCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			//b.CreateBracket(2);
			Assert.AreEqual(3, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_CreatesLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			//b.CreateBracket(2);
			Assert.AreEqual(4, b.NumberOfLowerRounds);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_CreatesLowerBracketMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			int numLowerMatches = 0;
			for (int r = 1; r <= b.NumberOfLowerRounds; ++r)
			{
				List<IMatch> round = b.GetLowerRound(r);
				numLowerMatches += round.Count;
			}
			Assert.AreEqual(6, numLowerMatches);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_AddsSingleElimPlayinRound_ForAbnormalSizedTournaments()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 15; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();
			int numLowerMatches = 0;
			for (int r = 1; r <= b.NumberOfLowerRounds; ++r)
			{
				List<IMatch> round = b.GetLowerRound(r);
				numLowerMatches += round.Count;
			}

			List<IPlayer> pList2 = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList2.Add(moq.Object);
			}
			IBracket b2 = new DoubleElimBracket(pList2);
			//b.CreateBracket();
			int numSecondLowerMatches = 0;
			for (int r = 1; r <= b2.NumberOfLowerRounds; ++r)
			{
				List<IMatch> round = b2.GetLowerRound(r);
				numSecondLowerMatches += round.Count;
			}

			Assert.AreEqual(numLowerMatches, numSecondLowerMatches);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_DoesNotAdvanceLosersFromPlayinRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 15; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			Assert.AreEqual(-1, b.GetMatch(1).NextLoserMatchNumber);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEB_CreateBracket_MakesGrandFinalMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsInstanceOfType(b.GrandFinal, typeof(IMatch));
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEBCreateBracket_AssignsMatchNumbersToLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();
			
			Assert.AreEqual(b.GetRound(b.NumberOfRounds)[0].MatchNumber + 1,
				b.GetLowerRound(1)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB CreateBracket")]
		public void DEBCreateBracket_AssignsMatchNumberToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.NumberOfMatches, b.GrandFinal.MatchNumber);
		}
#endregion

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void DEBNumberOfLowerRounds_ReturnsCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(4, b.NumberOfLowerRounds);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("Bracket Accessors")]
		public void DEBGetLowerRound_ReturnsCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(2, b.GetLowerRound(1).Count);
		}

#region Bracket Progression
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEB_AddGame_MovesLoserToLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(2, 0, 10, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Defender],
				b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		[TestCategory("Rankings")]
		public void DEBAddGame_UpperBracketWins_DoNotAffectRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= 3; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		[TestCategory("Rankings")]
		public void DEBAddGame_LowerBracketWins_AddsLoserToRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= 4; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.GetMatch(4).Players[(int)PlayerSlot.Challenger].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		[TestCategory("Rankings")]
		public void DEBAddGame_GrandFinalWin_AddsLoserToRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.GrandFinal.Players[(int)PlayerSlot.Challenger].Id,
				b.Rankings[1].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		[TestCategory("Rankings")]
		public void DEBAddGame_GrandFinalWin_AddsWinnerToRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.GrandFinal.Players[(int)PlayerSlot.Defender].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_MovesTeamsThroughLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);

			b.AddGame(b.GetLowerRound(1)[0].MatchNumber, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Defender],
				b.GetLowerRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_DropsLoserFromSecondRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);

			b.AddGame(3, 1, 0, PlayerSlot.Defender);
			Assert.AreEqual(b.GetMatch(3).Players[(int)PlayerSlot.Challenger],
				b.GetLowerRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_MovesUpperWinnerToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);

			b.AddGame(3, 0, 1, PlayerSlot.Challenger);
			Assert.AreEqual(b.GetMatch(3).Players[(int)PlayerSlot.Challenger],
				b.GrandFinal.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_MovesLowerWinnerToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for(int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.GetMatch(5).Players[(int)PlayerSlot.Defender],
				b.GrandFinal.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_AddsScoreToGrandFinalMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(1, b.GrandFinal.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddGame")]
		public void DEBAddGame_AddsGamesToGrandFinal_GamesList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(1, b.GrandFinal.Games.Count);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		public void DEBRemoveLastGame_RemovesFromLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			
			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(2, 1, 0, PlayerSlot.Defender);
			int mNum = b.GetLowerRound(1)[0].MatchNumber;
			b.GetMatch(mNum).SetMaxGames(3);
			b.AddGame(mNum, 1, 0, PlayerSlot.Defender);

			b.RemoveLastGame(mNum);
			Assert.AreEqual(0, b.GetMatch(mNum).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		[TestCategory("Rankings")]
		public void DEBRemoveLastGame_CorrectlyRemovesPriorLoserFromRankingsList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n <= 4; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.RemoveLastGame(4);
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		[TestCategory("Rankings")]
		public void DEBRemoveLastGame_UpdatesALLRankings_WhenMatchWinIsReversed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.RemoveLastGame(b.GetLowerRound(1)[0].MatchNumber);
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		public void DEBRemoveLastGame_RemovesFromGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.RemoveLastGame(b.GrandFinal.MatchNumber);
			Assert.AreEqual(0, b.GrandFinal.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void DEBRemoveLastGame_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.RemoveLastGame(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemoveLastGame")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void DEBRemoveLastGame_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.RemoveLastGame(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		public void DEBResetMatch_Resets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.GetMatch(1).SetMaxGames(3);
			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.AddGame(1, 0, 1, PlayerSlot.Challenger);

			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		public void DEBResetMatch_ResetsInLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.GetMatch(n).SetMaxGames(3);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(4);
			Assert.AreEqual(0, b.GetMatch(4).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		public void DEBResetMatch_ResetsALargeBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 11; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.GetMatch(n).SetMaxGames(3);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(2);
			Assert.AreEqual(0, b.GetMatch(2).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		public void DEBResetMatch_ResetsGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.GetMatch(n).SetMaxGames(3);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
				b.AddGame(n, 0, 1, PlayerSlot.Challenger);
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(b.GrandFinal.MatchNumber);
			Assert.AreEqual(0, b.GrandFinal.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatch")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		[TestCategory("Rankings")]
		public void DEBResetMatch_UpdatesALLRankings_WhenMatchWinIsReversed()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(1);
			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void DEBResetScore_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.ResetMatchScore(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatchScore")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void DEBResetScore_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.ResetMatchScore(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		public void DEB_RPFFM_RemovesPlayerFromLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.AddGame(1, 1, 0, PlayerSlot.Defender);
			b.RemoveLastGame(1);
			Assert.IsNull(b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		public void DEB_RPFFM_RemovesPlayerFromManyMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(b.GetLowerRound(1)[0].MatchNumber);
			Assert.IsNull(b.GrandFinal.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		public void DEB_RPFFM_ResetsFutureScores()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(b.GetLowerRound(1)[0].MatchNumber);
			Assert.AreEqual(0, b.GetLowerRound(2)[0].Score[0]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB RemovePlayerFromFutureMatches")]
		public void DEB_RPFFM_OnlyRemovesCorrectPlayersFromFuture()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}

			b.ResetMatchScore(1);
			Assert.IsNotNull(b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
#endregion

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		public void DEBSMGFWR_UpdatesCorrectUpperBracketRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			int games = 5;
			int round = 2;
			b.SetMaxGamesForWholeRound(round, games);
			Assert.AreEqual(games, b.GetRound(round)[1].MaxGames);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		public void DEBSMGFWR_DoesNotUpdateLowerBracketRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			int round = 2;
			b.SetMaxGamesForWholeRound(round, 5);
			Assert.AreEqual(1, b.GetLowerRound(round)[0].MaxGames);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("SetMaxGamesForWholeLowerRound")]
		public void DEBSMGFWLR_CorrectlyUpdatesLowerBracketRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			int round = 2;
			int games = 5;
			b.SetMaxGamesForWholeLowerRound(round, games);
			Assert.AreEqual(games, b.GetLowerRound(round)[1].MaxGames);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("SetMaxGamesForWholeRound")]
		public void DEBSMGFWLR_DoesNotUpdateUpperBracketRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 16; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			int round = 2;
			b.SetMaxGamesForWholeLowerRound(round, 5);
			Assert.AreEqual(1, b.GetRound(round)[0].MaxGames);
		}
	}
}
