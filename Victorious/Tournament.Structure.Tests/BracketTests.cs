using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class BracketTests
	{
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void NumberOfPlayers_GetsCorrectNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(2, b.NumberOfPlayers());
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void AddPlayer_Adds()
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
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_OnTryingToAddTwice()
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
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(NullReferenceException))]
		public void AddPlayer_ThrowsNullRef_OnNullParam()
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
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void AddPlayer_ResetsBracketMatches()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 2; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(p);

			Assert.AreEqual(0, b.NumberOfRounds());
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void ReplacePlayer_Replaces()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.ReplacePlayer(p, 2);

			Assert.AreEqual(p, b.Players[2]);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(NullReferenceException))]
		public void ReplacePlayer_ThrowsNullRef_OnNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.ReplacePlayer(null, 2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void ReplacePlayer_ThrowsOutOfRange_WithBadIndexParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.ReplacePlayer(new Mock<IPlayer>().Object, 6);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void RemovePlayer_Removes()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			b.AddPlayer(p);
			b.RemovePlayer(p);

			Assert.IsFalse(b.Players.Contains(p));
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(NullReferenceException))]
		public void RemovePlayer_ThrowsNullRef_OnNullParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.RemovePlayer(null);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RemovePlayer_ThrowsKeyNotFound_IfPlayerIsntPresent()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.RemovePlayer(new Mock<IPlayer>().Object);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void RemovePlayer_ClearsBracketMatches()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.AddPlayer(p);
			b.RemovePlayer(p);

			Assert.AreEqual(0, b.NumberOfRounds());
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void NumberOfRounds_ReturnsNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(2, b.NumberOfRounds());
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void GetRound_ReturnsCorrectMatchList()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);

			Assert.AreEqual(4, b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void GetRound_ThrowsOutOfRange_WithBadParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 8; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var l = b.GetRound(-1);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void GetMatch_ReturnsCorrectMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			IMatch m = b.GetMatch(2);

			Assert.AreEqual(1, m.DefenderIndex());
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void GetMatch_ThrowsOutOfRange_WithNegativeParam()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var m = b.GetMatch(-1);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void GetMatch_ThrowsNotFound_IfMatchNumberNotFound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			var m = b.GetMatch(15);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Bracket")]
		[TestCategory("Bracket Methods")]
		public void ResetBracket_Resets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 0; i < 4; ++i)
			{
				pList.Add(new Mock<IPlayer>().Object);
			}
			IBracket b = new SingleElimBracket(pList);
			b.ResetBracket();

			Assert.AreEqual(0, b.NumberOfRounds());
		}
	}
}
