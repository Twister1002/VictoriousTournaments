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
			IBracket b = new DoubleElimBracket();

			Assert.IsInstanceOfType(b, typeof(DoubleElimBracket));
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Ctor")]
		public void DEB_InheritsFieldsFromSEB()
		{
			IBracket b = new DoubleElimBracket();

			Assert.AreEqual(0, b.Players.Count);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_CreateBracket_InheritsUpperBracketCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			b.CreateBracket(2);

			Assert.AreEqual(3, b.Rounds.Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_CreateBracket_CreatesLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			b.CreateBracket(2);

			int numLowerRounds = (b is DoubleElimBracket) ? (b as DoubleElimBracket).LowerRounds.Count : 0;

			Assert.AreEqual(4, numLowerRounds);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("DoubleElimBracket Methods")]
		public void DEB_CreateBracket_CreatesLowerBracketMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			b.CreateBracket(2);

			int numFirstRoundMatches = (b is DoubleElimBracket)
				? (b as DoubleElimBracket).LowerRounds[(b as DoubleElimBracket).LowerRounds.Count - 1].Count
				: 0;

			Assert.AreEqual(2, numFirstRoundMatches);
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
			b.CreateBracket();

			int pIndex = b.Rounds[2][1].PlayerIndexes[0];
			b.AddWin(b.Rounds[2][1], 1);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).LowerRounds[3][0].PlayerIndexes[1]);
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
			b.CreateBracket();

			b.AddWin(b.Rounds[2][0], 0);
			b.AddWin(b.Rounds[2][1], 0);
			int pIndex = (b is DoubleElimBracket) ? (b as DoubleElimBracket).LowerRounds[3][0].PlayerIndexes[0] : 0;
			b.AddWin((b as DoubleElimBracket).LowerRounds[3][0], 0);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).LowerRounds[2][0].PlayerIndexes[0]);
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
			b.CreateBracket();

			b.AddWin(b.Rounds[1][0], 0);
			b.AddWin(b.Rounds[1][1], 0);
			int pIndex = b.Rounds[0][0].PlayerIndexes[1];
			b.AddWin(b.Rounds[0][0], 0);

			Assert.AreEqual(pIndex, (b as DoubleElimBracket).LowerRounds[0][0].PlayerIndexes[0]);
		}
	}
}
