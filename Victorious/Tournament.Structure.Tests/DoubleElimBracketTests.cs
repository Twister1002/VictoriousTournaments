using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class DoubleElimBracketTests
	{
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Ctor")]
		public void DEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			pList.Add(new Mock<IPlayer>().Object);
			pList.Add(new Mock<IPlayer>().Object);
			IBracket b = new DoubleElimBracket(pList);

			Assert.IsInstanceOfType(b, typeof(DoubleElimBracket));
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Ctor")]
		public void DEBCtor_InheritsFieldsFromSEB()
		{
			List<IPlayer> pList = new List<IPlayer>();
			pList.Add(new Mock<IPlayer>().Object);
			pList.Add(new Mock<IPlayer>().Object);
			IBracket b = new DoubleElimBracket(pList);

			Assert.AreEqual(2, b.Players.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void DEBCtor_ThrowsNullRef_WithNullParam()
		{
			IBracket b = new DoubleElimBracket(null);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void DEBCreateBracket_ThrowsOutOfRange_WithLessThanTwoPlayers()
		{
			IBracket b = new DoubleElimBracket();

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_InheritsUpperBracketCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			Assert.AreEqual(3, b.Rounds.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_CreatesLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			int numLowerRounds = (b is DoubleElimBracket)
				? (b as DoubleElimBracket).LowerRounds.Count : 0;

			Assert.AreEqual(4, numLowerRounds);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_CreatesLowerBracketMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			int numFirstRoundMatches = (b is DoubleElimBracket)
				? (b as DoubleElimBracket).GetLowerRound((b as DoubleElimBracket).LowerRounds.Count - 1).Count
				: 0;

			Assert.AreEqual(2, numFirstRoundMatches);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_AddsSingleElimPlayinRound_ForAbnormalSizedTournaments()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 15; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			int numFirstRoundMatches = (b is DoubleElimBracket)
				? (b as DoubleElimBracket).GetLowerRound((b as DoubleElimBracket).LowerRounds.Count - 1).Count
				: 0;

			Assert.AreEqual(2, numFirstRoundMatches);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_DoesNotAdvanceLosersFromPlayinRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 15; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket(2);

			Assert.AreEqual(-1, b.GetMatch(b.Rounds.Count - 1, 0).NextLoserMatchNumber);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEB_CreateBracket_MakesGrandFinalMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsNotNull((b as DoubleElimBracket).GrandFinal);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_MovesLoserToLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			int pIndex = b.GetMatch(2, 1).PlayerIndexes[0];
			b.AddWin(b.GetMatch(2, 1), 1);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).GetLowerMatch(3, 0).PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_MovesTeamsThroughLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.GetMatch(2, 0), 0);
			b.AddWin(b.GetMatch(2, 1), 0);
			int pIndex = (b is DoubleElimBracket)
				? (b as DoubleElimBracket).GetLowerMatch(3, 0).PlayerIndexes[0] : 0;
			b.AddWin((b as DoubleElimBracket).GetLowerMatch(3, 0), 0);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).GetLowerMatch(2, 0).PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_DropsLoserFromSecondRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.GetMatch(1, 0), 0);
			b.AddWin(b.GetMatch(1, 1), 0);
			int pIndex = b.GetMatch(0, 0).PlayerIndexes[1];
			b.AddWin(b.GetMatch(0, 0), 0);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).GetLowerMatch(0, 0).PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_MovesUpperWinnerToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.GetMatch(1, 0), 0);
			b.AddWin(b.GetMatch(1, 1), 0);
			int pIndex = b.GetMatch(0, 0).PlayerIndexes[1];
			b.AddWin(b.GetMatch(0, 0), 1);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).GrandFinal.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_MovesLowerWinnerToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.GetMatch(1, 0), 0);
			b.AddWin(b.GetMatch(1, 1), 0);
			b.AddWin(b.GetMatch(0, 0), 0);
			b.AddWin((b as DoubleElimBracket).GetLowerMatch(1, 0), 0);
			int pIndex = (b as DoubleElimBracket).GetLowerMatch(0, 0).PlayerIndexes[0];
			b.AddWin((b as DoubleElimBracket).GetLowerMatch(0, 0), 0);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).GrandFinal.PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_AddWin_AddsWinsToGrandFinalMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(b.GetMatch(1, 0), 0);
			b.AddWin(b.GetMatch(1, 1), 0);
			b.AddWin(b.GetMatch(0, 0), 0);
			b.AddWin((b as DoubleElimBracket).GetLowerMatch(1, 0), 0);
			b.AddWin((b as DoubleElimBracket).GetLowerMatch(0, 0), 0);
			b.AddWin((b as DoubleElimBracket).GrandFinal, 0);

			Assert.AreEqual(1, (b as DoubleElimBracket).GrandFinal.Score[0]);
		}
	}
}
