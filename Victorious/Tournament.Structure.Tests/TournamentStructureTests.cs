using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

using Tournament.Structure;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class TournamentStructureTests
	{
		#region Tournament Tests
		[TestMethod]
		[TestCategory("Tournament")]
		[TestCategory("Tournament Constructor")]
		public void DefaultCtor_Constructs()
		{
			Tournament t = new Tournament();

			Assert.AreEqual(false, t.IsPublic);
		}
		[TestMethod]
		[TestCategory("Tournament")]
		[TestCategory("Tournament Constructor")]
		public void FullCtor_Constructs()
		{
			string str = "title";
			List<IPlayer> pList = new List<IPlayer>();
			//pList.Add(new Mock<IPlayer>().Object);
			List<IBracket> bList = new List<IBracket>();
			//bList.Add(new Mock<IBracket>().Object);
			Tournament t = new Tournament(str, pList, bList, 1.0f, true);

			Assert.AreEqual(str, t.Title);
		}
		#endregion

		#region Player Tests
		//[TestMethod]
		//[TestCategory("Player")]
		//[TestCategory("Player Constructor")]
		//public void PlayerCtor_Constructs()
		//{
		//	string un = "username";
		//	IPlayer p = new Player(un, "firstname", "lastname", "email");

		//	Assert.AreEqual(un, p.Username);
		//}
		#endregion

		#region Match Tests
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		public void MatchCtor_Constructs()
		{
			Match m = new Match();

			Assert.AreEqual(1, m.WinsNeeded);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		public void MatchOverloadedCtor_Constructs()
		{
			ushort[] sc = new ushort[2] { 1, 1 };
			Match m = new Match(2, new int[2] { 0, 1 }, sc, 1, 1, new List<int>(), 0);

			Assert.AreEqual(sc, m.Score);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsAPlayer()
		{
			int pIndex1 = 10;
			int pIndex2 = 20;
			Match m = new Match();
			m.AddPlayer(pIndex1);
			m.AddPlayer(pIndex2);

			Assert.AreEqual(pIndex1, m.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsTwoPlayers()
		{
			int pIndex1 = 10;
			int pIndex2 = 20;
			Match m = new Match();
			m.AddPlayer(pIndex1);
			m.AddPlayer(pIndex2);

			Assert.AreEqual(pIndex2, m.PlayerIndexes[1]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayer_Removes()
		{
			int pIndex = 10;
			Match m = new Match();
			m.AddPlayer(pIndex);
			m.RemovePlayer(pIndex);

			Assert.AreEqual(-1, m.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayers_ResetsArr1()
		{
			Match m = new Match();
			m.AddPlayer(10);
			m.AddPlayer(20);
			m.RemovePlayers();

			Assert.AreEqual(-1, m.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayers_ResetsArr2()
		{
			Match m = new Match();
			m.AddPlayer(10);
			m.AddPlayer(20);
			m.RemovePlayers();

			Assert.AreEqual(-1, m.PlayerIndexes[1]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddWin_AddsAWin()
		{
			Match m = new Match(3, new int[2] { 0, 1 }, new ushort[2] { 1, 0 }, 0, 0, new List<int>(), 0);
			m.AddWin(0);
			m.AddWin(0);

			Assert.AreEqual(1 + 1 + 1, m.Score[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddWin_Fails_OverMaxValue()
		{
			Match m = new Match();
			m.AddWin(0);

			Assert.IsFalse(m.AddWin(0));
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPrevMatchIndex_Adds()
		{
			int i = 14;
			Match m = new Match();
			m.AddPrevMatchIndex(i);

			Assert.IsTrue(m.PrevMatchIndexes.Contains(i));
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPrevMatchIndex_FailsAfterTwoCalls()
		{
			Match m = new Match();
			m.AddPrevMatchIndex(0);
			m.AddPrevMatchIndex(1);

			Assert.IsFalse(m.AddPrevMatchIndex(2));
		}
		#endregion

		#region SingleElimBracket Tests
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Ctor")]
		public void SEBCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			Bracket b = new SingleElimBracket(pList);

			Assert.AreEqual(pList, b.Players);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_CreatesFor4Players()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.AreEqual(2, b.Rounds[1].Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_4Players_DoesNotAssignToRound0()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.AreEqual(-1, b.Rounds[0][0].PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_4Players_AssignsToRound1()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.AreEqual(3, b.Rounds[1][0].PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_5Players_1FirstRoundMatch_and_2SecondRoundMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.IsTrue(b.Rounds[2].Count == 1
				&& b.Rounds[1].Count == 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_5Players_CorrectlyAssignsBye()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 5; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.AreEqual(-1, b.Rounds[1][0].PlayerIndexes[1]);
			// PlayerIndex==-1 is (no player), implying prev round's winner
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyGeneratesMatches()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.IsTrue(5 == b.Rounds.Count
				&& 13 == b.Rounds[b.Rounds.Count - 1].Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		public void SEBCreateBracket_29Players_CorrectlyAssignsPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 29; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			Assert.IsTrue(15 == b.Rounds[b.Rounds.Count - 1][0].PlayerIndexes[0]
				&& 16 == b.Rounds[b.Rounds.Count - 1][0].PlayerIndexes[1]);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddPlayer_Adds()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			Bracket b = new SingleElimBracket();
			b.AddPlayer(p);

			Assert.IsTrue(b.Players.Contains(p));
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddPlayer_FailsOnDuplicates()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			Bracket b = new SingleElimBracket();
			b.AddPlayer(p);

			Assert.IsFalse(b.AddPlayer(p));
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddRound_Adds()
		{
			Bracket b = new SingleElimBracket(null);
			b.AddRound();
			b.AddRound();

			Assert.AreEqual(2, b.Rounds.Count);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddMatch_Blank_Adds()
		{
			Bracket b = new SingleElimBracket(null);
			b.AddRound();
			b.AddMatch(0);
			b.AddMatch(0);

			Assert.AreEqual(2, b.Rounds[0].Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddMatch_Adds()
		{
			Bracket b = new SingleElimBracket(null);
			b.AddRound();
			b.AddMatch(0, new Mock<IMatch>().Object);
			b.AddMatch(0, new Mock<IMatch>().Object);

			Assert.AreEqual(2, b.Rounds[0].Count);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddMatch_FailsWithOutOfRangeParam()
		{
			Bracket b = new SingleElimBracket(null);

			Assert.IsFalse(b.AddMatch(0, new Mock<IMatch>().Object));
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddWin_4Players_UpdatesScore()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			b.AddWin(1, 0, 1);

			Assert.AreEqual(1, b.Rounds[1][0].Score[1]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddWin_4Players_DoesNotUpdatePastMatchWin()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			b.AddWin(1, 0, 1);
			b.AddWin(1, 0, 1); // does nothing

			Assert.AreEqual(1, b.Rounds[1][0].Score[1]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		public void SEBAddWin_4Players_AdvancesWinningPlayer()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			Bracket b = new SingleElimBracket(pList);
			b.CreateBracket();

			int playerNum = b.Rounds[1][1].PlayerIndexes[1];
			b.AddWin(1, 1, 1);

			Assert.AreEqual(playerNum, b.Rounds[0][0].PlayerIndexes[1]);
		}
		#endregion
	}
}
