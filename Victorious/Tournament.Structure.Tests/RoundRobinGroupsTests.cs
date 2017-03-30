using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class RoundRobinGroupsTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		public void RRGCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			Assert.IsInstanceOfType(b, typeof(RoundRobinGroups));
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void RRGCtor_ThrowsNullException()
		{
			IBracket b = new RoundRobinGroups(null, 2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RRGCtor_ThrowsOutOfRange_WithTooFewGroups()
		{
			IBracket b = new RoundRobinGroups(new List<IPlayer>(), 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void RRGCtor_ThrowsOutOfRange_WithTooManyGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, pList.Count);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG CreateBracket")]
		public void RRGCreateBracket_CreatesFourGroups()
		{
			int numGroups = 4;
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, numGroups);

			Assert.AreEqual((4 * 7 * 4), b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG CreateBracket")]
		public void RRGCreateBracket_CreatesUnevenGroupsForOddPlayercounts()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 7; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			Assert.AreEqual((2 * 3 + 3), b.NumberOfMatches);
		}
#if false
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Ctor")]
		public void RRGCtor_IntCtorConstructs()
		{
			int numPlayers = 4;
			IBracket b = new RoundRobinGroups(numPlayers, 2);

			Assert.AreEqual(numPlayers, b.Players.Count);
		}
#endif
		#endregion

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		public void RRGGetMatch_ReturnsMatchesFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			int getMatchNumber = 30;
			IMatch m = b.GetMatch(getMatchNumber);

			Assert.AreNotEqual(getMatchNumber, m.MatchNumber);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRGGetMatch_ThrowsInvalidIndex_WithLowMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			var m = b.GetMatch(-1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		[ExpectedException(typeof(MatchNotFoundException))]
		public void RRGGetMatch_ThrowsNotFound_WithHighMatchNumber()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.GetMatch(b.NumberOfMatches + 1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		public void RRGGetRound_ReturnsMatchesFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 32; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 4);

			Assert.AreEqual((4 * 4), b.GetRound(1).Count);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		[ExpectedException(typeof(NullReferenceException))]
		public void RRGGetRound_ThrowsNullRef_WithNoGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);
			b.RemovePlayer(pList[0]);

			b.GetRound(1);
			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG Overloaded Accessors")]
		[ExpectedException(typeof(InvalidIndexException))]
		public void RRGGetRound_ThrowsInvalidIndex_WithNegativeRound()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.GetRound(-1);
			Assert.AreEqual(1, 2);
		}

		#region Bracket Progression
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddWin")]
		public void RRGAddWin_AddsWinToFirstGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);
			b.AddWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(1, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddWin")]
		public void RRGAddWin_AddsWinToAnyGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddWin(matchNum, PlayerSlot.Challenger);

			Assert.AreEqual(1, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG AddWin")]
		public void RRGAddWin_CanFinishBracket()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddWin(n, PlayerSlot.Defender);
			}

			Assert.IsTrue(b.IsFinished);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG SubtractWin")]
		public void RRGSubtractWin_Subtracts()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.AddWin(1, PlayerSlot.Challenger);
			b.SubtractWin(1, PlayerSlot.Challenger);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG SubtractWin")]
		public void RRGSubtractWin_SubtractsFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddWin(matchNum, PlayerSlot.Challenger);
			b.SubtractWin(matchNum, PlayerSlot.Challenger);

			Assert.AreEqual(0, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}

		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatchScore")]
		public void RRGResetMatchScore_Resets()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			b.AddWin(1, PlayerSlot.Challenger);
			b.ResetMatchScore(1);

			Assert.AreEqual(0, b.GetMatch(1).Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("RoundRobinGroups")]
		[TestCategory("RRG ResetMatchScore")]
		public void RRGResetMatchScore_ResetsFromAllGroups()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 4; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new RoundRobinGroups(pList, 2);

			int matchNum = b.NumberOfMatches;
			b.AddWin(matchNum, PlayerSlot.Challenger);
			b.ResetMatchScore(matchNum);

			Assert.AreEqual(0, b.GetMatch(matchNum).Score[(int)PlayerSlot.Challenger]);
		}
		#endregion
	}
}
