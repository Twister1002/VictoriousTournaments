using System;
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
		[TestCategory("SingleElimBracket Ctor")]
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
		[TestCategory("SingleElimBracket Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBCtor_ThrowsNullRef_WithNullParam()
		{
			IBracket b = new SingleElimBracket(null);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("CreateBracket")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SEBCreateBracket_ThrowsOutOfRange_WithLessThanTwoPlayers()
		{
			IBracket b = new SingleElimBracket();
			//b.CreateBracket();

			Assert.AreEqual(1, 2);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(2, b.GetRound(1).Count);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();
			
			Assert.AreEqual(-1, b.GetMatch(0, 0).PlayerIndexes[0]);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();
			
			Assert.AreEqual(3, b.GetMatch(1, 0).PlayerIndexes[1]);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue
				(b.GetRound(2).Count == 1
				&& b.GetRound(1).Count == 2);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.AreEqual(-1, b.GetMatch(1, 0).PlayerIndexes[1]);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(5 == b.Rounds.Count
				&& 13 == b.GetRound(b.Rounds.Count - 1).Count);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			Assert.IsTrue(15 == b.GetMatch(b.Rounds.Count - 1, 0).PlayerIndexes[0]
				&& 16 == b.GetMatch(b.Rounds.Count - 1, 0).PlayerIndexes[1]);
		}

		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Methods")]
		public void SEBAddPlayer_Adds()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(p);

			Assert.IsTrue(b.Players.Contains(p));
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void SEBAddPlayer_ThrowsDuplicate_OnTryingToAddTwice()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(p);
			b.AddPlayer(p);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(NullReferenceException))]
		public void SEBAddPlayer_ThrowsNullRef_OnNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(null);

			Assert.AreEqual(1, 2);
		}

		//[TestMethod]
		//[TestCategory("SingleElimBracket")]
		//[TestCategory("Bracket Methods")]
		//public void SEBAddRound_Adds()
		//{
		//	IBracket b = new SingleElimBracket(null);
		//	b.AddRound();
		//	b.AddRound();

		//	Assert.AreEqual(2, b.Rounds.Count);
		//}

		//[TestMethod]
		//[TestCategory("SingleElimBracket")]
		//[TestCategory("SingleElimBracket Methods")]
		//public void SEBAddMatch_Blank_Adds()
		//{
		//	IBracket b = new SingleElimBracket(null);
		//	b.AddRound();
		//	b.AddMatch(0);
		//	b.AddMatch(0);

		//	Assert.AreEqual(2, b.Rounds[0].Count);
		//}
		//[TestMethod]
		//[TestCategory("SingleElimBracket")]
		//[TestCategory("SingleElimBracket Methods")]
		//public void SEBAddMatch_Adds()
		//{
		//	IBracket b = new SingleElimBracket(null);
		//	b.AddRound();
		//	b.AddMatch(0, new Mock<IMatch>().Object);
		//	b.AddMatch(0, new Mock<IMatch>().Object);

		//	Assert.AreEqual(2, b.Rounds[0].Count);
		//}
		//[TestMethod]
		//[TestCategory("SingleElimBracket")]
		//[TestCategory("SingleElimBracket Methods")]
		//public void SEBAddMatch_FailsWithOutOfRangeParam()
		//{
		//	IBracket b = new SingleElimBracket(null);

		//	Assert.IsFalse(b.AddMatch(0, new Mock<IMatch>().Object));
		//}

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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(1, 0, 1);

			Assert.AreEqual(1, b.GetMatch(1, 0).Score[1]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
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

			b.AddWin(1, 0, 1); // ends match
			b.AddWin(1, 0, 1); // throws exception

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SEBAddWin_ThrowsOutOfRange_WithBadParams()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			b.AddWin(-1, 0, 1);

			Assert.AreEqual(1, 2);
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
			IBracket b = new SingleElimBracket(pList);
			//b.CreateBracket();

			int playerNum = b.GetMatch(1, 1).PlayerIndexes[1];
			b.AddWin(1, 1, 1);

			Assert.AreEqual(playerNum, b.GetMatch(0, 0).PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("SingleElimBracket")]
		[TestCategory("SingleElimBracket Methods")]
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

			b.AddWin(new Mock<IMatch>().Object, 0);

			Assert.AreEqual(1, 2);
		}
	}
}
