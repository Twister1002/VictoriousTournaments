using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class DoubleElimBracketTests
	{
#if false
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
#if false
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void DEBCtor_ThrowsNullRef_WithNullParam()
		{
			IBracket b = new DoubleElimBracket(null);

			Assert.AreEqual(1, 2);
		}
#endif
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
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_MovesLoserToLowerBracket()
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

			b.AddWin(2, PlayerSlot.Challenger);

			Assert.AreEqual(b.GetMatch(2).Players[(int)PlayerSlot.Defender],
				b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		[TestCategory("Rankings")]
		public void DEBAddWin_UpperBracketWins_DoNotPopulateRankingsArray()
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
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		[TestCategory("Rankings")]
		public void DEBAddWin_LowerBracketWins_PopulateRankingsArray()
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
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.AreEqual(b.GetMatch(4).Players[(int)PlayerSlot.Challenger].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		[TestCategory("Rankings")]
		public void DEBAddWin_GrandFinalWin_AddsLoserToRankingsArray()
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
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.AreEqual(b.GrandFinal.Players[(int)PlayerSlot.Challenger].Id,
				b.Rankings[1].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		[TestCategory("Rankings")]
		public void DEBAddWin_GrandFinalWin_AddsWinnerToRankingsArray()
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
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.AreEqual(b.GrandFinal.Players[(int)PlayerSlot.Defender].Id,
				b.Rankings[0].Id);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_MovesTeamsThroughLowerBracket()
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(b.GetLowerRound(1)[0].MatchNumber, PlayerSlot.Defender);

			Assert.AreEqual(b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Defender],
				b.GetLowerRound(2)[0].Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_DropsLoserFromSecondRound()
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);

			Assert.AreEqual(b.GetMatch(3).Players[(int)PlayerSlot.Challenger],
				b.GetLowerRound(2)[0].Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_MovesUpperWinnerToGrandFinal()
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Challenger);

			Assert.AreEqual(b.GetMatch(3).Players[(int)PlayerSlot.Challenger],
				b.GrandFinal.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_MovesLowerWinnerToGrandFinal()
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(4, PlayerSlot.Defender);
			b.AddWin(5, PlayerSlot.Defender);

			Assert.AreEqual(b.GetMatch(5).Players[(int)PlayerSlot.Defender],
				b.GrandFinal.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB AddWin")]
		public void DEB_AddWin_AddsWinsToGrandFinalMatch()
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(4, PlayerSlot.Defender);
			b.AddWin(5, PlayerSlot.Defender);
			b.AddWin(b.GrandFinal.MatchNumber, PlayerSlot.Defender);

			Assert.AreEqual(1, b.GrandFinal.Score[(int)PlayerSlot.Defender]);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		public void DEBSubtractWin_Subtracts()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.GetMatch(1).SetWinsNeeded(2);
			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		public void DEBSubtractWin_SubtractsFromLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			int mNum = b.GetLowerRound(1)[0].MatchNumber;
			b.GetMatch(mNum).SetWinsNeeded(2);
			b.AddWin(mNum, PlayerSlot.Defender);
			b.SubtractWin(mNum, PlayerSlot.Defender);

			Assert.AreEqual(0, b.GetMatch(mNum).Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		[TestCategory("Rankings")]
		public void DEBSubtractWin_UpdatesRankings_WhenMatchWinIsReversed()
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
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(4, PlayerSlot.Defender);

			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		[TestCategory("Rankings")]
		public void DEBSubtractWin_UpdatesALLRankings_WhenMatchWinIsReversed()
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
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(4, PlayerSlot.Defender);

			Assert.AreEqual(0, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		public void DEBSubtractWin_SubtractsFromGrandFinal()
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
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(b.GrandFinal.MatchNumber, PlayerSlot.Defender);

			Assert.AreEqual(0, b.GrandFinal.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		public void DEBSubtractWin_DoesntIterateIfMatchDoesntReset()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			for (int n = 1; n < b.GrandFinal.MatchNumber; ++n)
			{
				b.GetMatch(n).SetWinsNeeded(2);
				b.AddWin(n, PlayerSlot.Defender);
				b.AddWin(n, PlayerSlot.Challenger);
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.SubtractWin(4, PlayerSlot.Challenger);

			Assert.IsNotNull(b.GetMatch(5).Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void DEBSubtractWin_ThrowsInvalidIndex_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.SubtractWin(-1, 0);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB SubtractWin")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void DEBSubtractWin_ThrowsNotFound_WithBadMatchNumberInput()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.SubtractWin(b.NumberOfMatches + 1, 0);
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

			b.GetMatch(1).SetWinsNeeded(2);
			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
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
				b.GetMatch(n).SetWinsNeeded(2);
				b.AddWin(n, PlayerSlot.Defender);
				b.AddWin(n, PlayerSlot.Challenger);
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.ResetMatchScore(4);

			Assert.AreEqual(0, b.GetMatch(4).Score[(int)PlayerSlot.Challenger]);
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
				b.GetMatch(n).SetWinsNeeded(2);
				b.AddWin(n, PlayerSlot.Defender);
				b.AddWin(n, PlayerSlot.Challenger);
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.ResetMatchScore(b.GrandFinal.MatchNumber);

			Assert.AreEqual(0, b.GrandFinal.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DEB ResetMatch")]
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
				b.AddWin(n, PlayerSlot.Defender);
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
		public void DEB_RPFFM_RemovesPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new DoubleElimBracket(pList);

			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

			Assert.IsNull(b.GetRound(2)[0].Players[(int)PlayerSlot.Defender]);
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

			b.AddWin(1, PlayerSlot.Defender);
			b.SubtractWin(1, PlayerSlot.Defender);

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
				b.AddWin(n, PlayerSlot.Defender);
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
				b.AddWin(n, PlayerSlot.Defender);
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
				b.AddWin(n, PlayerSlot.Defender);
			}
			b.ResetMatchScore(1);

			Assert.IsNotNull(b.GetLowerRound(1)[0].Players[(int)PlayerSlot.Challenger]);
		}
#endregion
#endif
	}
}
