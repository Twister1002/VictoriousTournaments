using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class MatchTests
	{
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Ctor")]
		public void MatchDefaultCtor_Constructs()
		{
			IMatch m = new Match();

			Assert.AreEqual(1, m.WinsNeeded);
		}
#if false
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Ctor")]
		public void MatchOverloadedCtor_Constructs()
		{
			ushort[] sc = new ushort[2] { 1, 1 };
			IMatch m = new Match(true, false, 2, new int[2] { 0, 1 }, -1, sc, 1, 1, 1, new List<int>(), 0, 0);

			Assert.AreEqual(sc, m.Score);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Ctor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void MatchOverloadedCtor_ThrowsNullRef_OnNullParams()
		{
			IMatch m = new Match(false, false, 2, null, -1, null, 0, 0, 0, null, 0, 0);

			Assert.AreEqual(1, 2);
		}
#endif

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_AddsAPlayer()
		{
			IPlayer p = new Mock<IPlayer>().Object;
			IMatch m = new Match();
			m.AddPlayer(p);

			Assert.AreEqual(p, m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_DoesNotActivateMatch()
		{
			IMatch m = new Match();
			m.AddPlayer(new Mock<IPlayer>().Object);

			Assert.AreEqual(false, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_AddsTwoPlayers()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			Assert.AreEqual(p2.Object, m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_SecondPlayerActivatesMatch()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);

			Assert.AreEqual(true, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		public void AddPlayer_AddsSecondPlayer()
		{
			var p2 = new Mock<IPlayer>();

			IMatch m = new Match();
			m.AddPlayer(p2.Object, PlayerSlot.Challenger);

			Assert.AreEqual(p2.Object, m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void AddPlayer_ThrowsException_WithBadIndexParam()
		{
			IMatch m = new Match();
			m.AddPlayer(new Mock<IPlayer>().Object, (PlayerSlot)4);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(SlotFullException))]
		public void AddPlayer_ThrowsException_WhenAddingPlayerToSameSpot()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.AddPlayer(p2.Object, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddPlayer")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_WhenAddingSamePlayerTwice()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(1);

			IMatch m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Challenger);
			m.AddPlayer(p1.Object, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		public void RemovePlayer_Removes()
		{
			int pIndex = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(pIndex);
			
			IMatch m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.RemovePlayer(pIndex);

			Assert.IsNull(m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		public void RemovePlayer_SetsMatchInactive()
		{
			int pIndex = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(pIndex);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.RemovePlayer(pIndex);

			Assert.AreEqual(false, m.IsReady);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RemovePlayer_ThrowsKeyNotFound_IfPlayerToRemoveDoesntExist()
		{
			IMatch m = new Match();
			m.RemovePlayer(10);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_ResetsArr1()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.IsNull(m.Players[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_ResetsArr2()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.IsNull(m.Players[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetPlayers")]
		public void ResetPlayers_SetsMatchInactive()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.ResetPlayers();

			Assert.AreEqual(false, m.IsReady);
		}
		
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		public void AddWin_AddsAWin()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1, m.Score[(int)PlayerSlot.Defender]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		public void AddWin_SetsMatchFinishedCorrectly()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);

			Assert.AreEqual(true, m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		public void AddWin_SetsWinnerIndex_IfMatchIsOver()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			PlayerSlot wSlot = PlayerSlot.Challenger;
			m.AddWin(wSlot);

			Assert.AreEqual(wSlot, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		public void AddWin_DoesNotSetMatchFinishedIncorrectly()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.SetWinsNeeded(2);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Challenger);

			Assert.AreEqual(false, m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void AddWin_ThrowsOutOfRange_WithBadInput()
		{
			IMatch m = new Match();
			m.AddWin((PlayerSlot)3);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_WithNotEnoughPlayersInMatch()
		{
			IMatch m = new Match();
			m.AddPlayer(new Mock<IPlayer>().Object);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match AddWin")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_IfMatchIsAlreadyWon()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match SubtractWin")]
		public void SubtractWin_Subtracts()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.SetWinsNeeded(3);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.AddWin(PlayerSlot.Challenger);
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(1, m.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match SubtractWin")]
		public void SubtractWin_SetsFinishedToFalse()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(false, m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match SubtractWin")]
		public void SubtractWin_ResetsWinnerIndex()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(PlayerSlot.unspecified, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match SubtractWin")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SubtractWin_ThrowsOutOfRange_WithBadInput()
		{
			IMatch m = new Match();
			m.SubtractWin((PlayerSlot)3);

			Assert.AreEqual(1, 2);
		}
#if false
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match SubtractWin")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SubtractWin_ThrowsException_WhenScoreIsZero()
		{
			IMatch m = new Match();
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(1, 2);
		}
#endif

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_ResetsToZero()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.SetWinsNeeded(2);
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(0, m.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_SetsMatchUnfinished()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(false, m.IsFinished);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match ResetScore")]
		public void ResetScore_ResetsWinnerIndex()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(PlayerSlot.unspecified, m.WinnerSlot);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match RemovePlayer")]
		[TestCategory("Match ResetScore")]
		public void RemovePlayer_ResetsScore()
		{
			int pIndex = 10;
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(pIndex);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object, PlayerSlot.Defender);
			m.AddPlayer(p2.Object, PlayerSlot.Challenger);
			m.AddWin(PlayerSlot.Defender);
			m.RemovePlayer(pIndex);

			Assert.AreEqual(0, m.Score[(int)PlayerSlot.Defender]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SetWinsNeeded_ThrowsOutOfRange_WithZeroInput()
		{
			IMatch m = new Match();
			m.SetWinsNeeded(0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void SetWinsNeeded_ThrowsInactive_WhenCalledOnFinishedMatch()
		{
			var p1 = new Mock<IPlayer>();
			p1.Setup(p => p.Id).Returns(10);
			var p2 = new Mock<IPlayer>();
			p2.Setup(p => p.Id).Returns(20);

			IMatch m = new Match();
			m.AddPlayer(p1.Object);
			m.AddPlayer(p2.Object);
			m.AddWin(PlayerSlot.Challenger);
			m.SetWinsNeeded(2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetRoundIndex_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			IMatch m = new Match();
			m.SetRoundIndex(2);
			m.SetRoundIndex(3);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetMatchIndex_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			IMatch m = new Match();
			m.SetMatchIndex(2);
			m.SetMatchIndex(3);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetMatchNumber_Sets()
		{
			int n = 5;
			IMatch m = new Match();
			m.SetMatchNumber(n);

			Assert.AreEqual(n, m.MatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			IMatch m = new Match();
			m.SetMatchNumber(1);
			m.SetMatchNumber(2);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void AddPreviousMatchNumber_Adds()
		{
			int i = 14;
			IMatch m = new Match();
			m.AddPreviousMatchNumber(i);
			m.AddPreviousMatchNumber(2);

			Assert.IsTrue(m.PreviousMatchNumbers.Contains(i));
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void AddPreviousMatchNumber_ThrowsAlreadyAssigned_AfterMoreThanTwoCalls()
		{
			IMatch m = new Match();
			m.AddPreviousMatchNumber(0);
			m.AddPreviousMatchNumber(1);
			m.AddPreviousMatchNumber(2);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetNextMatchNumber_Sets()
		{
			int n = 5;
			IMatch m = new Match();
			m.SetNextMatchNumber(n);

			Assert.AreEqual(n, m.NextMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetNextMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			IMatch m = new Match();
			m.SetNextMatchNumber(1);
			m.SetNextMatchNumber(2);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		public void SetNextLoserMatchNumber_Sets()
		{
			int n = 7;
			IMatch m = new Match();
			m.SetNextLoserMatchNumber(n);

			Assert.AreEqual(n, m.NextLoserMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Mutators")]
		[ExpectedException(typeof(AlreadyAssignedException))]
		public void SetNextLoserMatchNumber_ThrowsAlreadyAssigned_WhenCalledTwice()
		{
			IMatch m = new Match();
			m.SetNextLoserMatchNumber(1);
			m.SetNextLoserMatchNumber(2);

			Assert.AreEqual(1, 2);
		}
	}
}
