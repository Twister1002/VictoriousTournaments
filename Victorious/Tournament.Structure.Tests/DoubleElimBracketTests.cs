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

			Assert.AreEqual(3, b.NumberOfRounds());
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

			Assert.AreEqual(4, b.NumberOfLowerRounds());
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

			Assert.AreEqual(2, b.GetLowerRound(1).Count);
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
			//b.CreateBracket();

			List<IPlayer> pList2 = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList2.Add(new Mock<IPlayer>().Object);
			}
			IBracket b2 = new DoubleElimBracket(pList2);
			//b.CreateBracket();

			Assert.AreEqual(b.NumberOfLowerRounds(), b2.NumberOfLowerRounds());
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

			Assert.AreEqual(-1, b.GetMatch(1).NextLoserMatchNumber);
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

			Assert.IsNotNull(b.GetGrandFinal());
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEBCreateBracket_AssignsMatchNumbersToLowerBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.GetRound(2)[0].MatchNumber + 1, b.GetLowerRound(1)[0].MatchNumber);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("CreateBracket")]
		public void DEBCreateBracket_AssignsMatchNumberToGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.GetLowerRound(b.NumberOfLowerRounds())[0].MatchNumber + 1,
				b.GetGrandFinal().MatchNumber);
		}

		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("Bracket Methods")]
		public void DEBNumberOfLowerRounds_ReturnsCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(4, b.NumberOfLowerRounds());
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("Bracket Methods")]
		public void DEBGetLowerRound_ReturnsCorrectly()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(2, b.GetLowerRound(1).Count);
		}
		[TestMethod]
		[TestCategory("DoubleElimBracket")]
		[TestCategory("Bracket Methods")]
		public void DEBGetGrandFinal_ReturnsGrandFinal()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new DoubleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(b.GetRound(2)[0].MatchNumber,
				b.GetGrandFinal().PreviousMatchNumbers[0]);
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

			int pIndex = b.GetMatch(2).DefenderIndex();
			b.AddWin(2, PlayerSlot.Challenger);

			Assert.AreEqual(pIndex, b.GetLowerRound(1)[0].ChallengerIndex());
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			int pIndex = b.GetLowerRound(1)[0].DefenderIndex();
			b.AddWin(b.GetLowerRound(1)[0].MatchNumber, PlayerSlot.Defender);

			Assert.AreEqual(pIndex, b.GetLowerRound(2)[0].ChallengerIndex());
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			int pIndex = b.GetMatch(3).ChallengerIndex();
			b.AddWin(3, PlayerSlot.Defender);

			Assert.AreEqual(pIndex, b.GetLowerRound(2)[0].DefenderIndex());
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			int pIndex = b.GetMatch(3).ChallengerIndex();
			b.AddWin(3, PlayerSlot.Challenger);

			Assert.AreEqual(pIndex, b.GetGrandFinal().DefenderIndex());
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(4, PlayerSlot.Defender);
			int pIndex = b.GetMatch(5).DefenderIndex();
			b.AddWin(5, PlayerSlot.Defender);

			Assert.AreEqual(pIndex, b.GetGrandFinal().ChallengerIndex());
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

			b.AddWin(1, PlayerSlot.Defender);
			b.AddWin(2, PlayerSlot.Defender);
			b.AddWin(3, PlayerSlot.Defender);
			b.AddWin(4, PlayerSlot.Defender);
			b.AddWin(5, PlayerSlot.Defender);
			b.AddWin(b.GetGrandFinal().MatchNumber, PlayerSlot.Defender);

			Assert.AreEqual(1, b.GetGrandFinal().Score[(int)PlayerSlot.Defender]);
		}
	}
}
