using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class MatchTests
	{
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		public void MatchDefaultCtor_Constructs()
		{
			IMatch m = new Match();

			Assert.AreEqual(1, m.WinsNeeded);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		public void MatchOverloadedCtor_Constructs()
		{
			ushort[] sc = new ushort[2] { 1, 1 };
			IMatch m = new Match(2, new int[2] { 0, 1 }, sc, 1, 1, 1, new List<int>(), 0, 0);

			Assert.AreEqual(sc, m.Score);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void MatchOverloadedCtor_ThrowsNullRef_OnNullParams()
		{
			IMatch m = new Match(2, null, null, 0, 0, 0, null, 0, 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Constructor")]
		[TestCategory("Models")]
		[ExpectedException(typeof(NullReferenceException))]
		public void MatchModelCtor_ThrowsNullRef_OnNullParams()
		{
			IMatch m = new Match(null, null);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsAPlayer()
		{
			int pIndex = 5;
			IMatch m = new Match();
			m.AddPlayer(pIndex);

			Assert.AreEqual(pIndex, m.DefenderIndex());
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsTwoPlayers()
		{
			int pIndex1 = 10;
			int pIndex2 = 20;
			IMatch m = new Match();
			m.AddPlayer(pIndex1);
			m.AddPlayer(pIndex2);

			Assert.AreEqual(pIndex2, m.ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsSecondPlayer()
		{
			int pIndex2 = 20;
			IMatch m = new Match();
			m.AddPlayer(pIndex2, PlayerSlot.Challenger);

			Assert.AreEqual(pIndex2, m.ChallengerIndex());
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void AddPlayer_ThrowsException_WithBadIndexParam()
		{
			IMatch m = new Match();
			m.AddPlayer(1, (PlayerSlot)4);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(SlotFullException))]
		public void AddPlayer_ThrowsException_WhenAddingPlayerToSameSpot()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Defender);
			m.AddPlayer(20, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_WhenAddingSamePlayerTwice()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Challenger);
			m.AddPlayer(10, PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayer_Removes()
		{
			int pIndex = 10;
			IMatch m = new Match();
			m.AddPlayer(pIndex, 0); // 0 = PlayerSlot.Defender
			m.RemovePlayer(pIndex);

			Assert.AreEqual(-1, m.DefenderIndex());
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(KeyNotFoundException))]
		public void RemovePlayer_ThrowsKeyNotFound_IfPlayerToRemoveDoesntExist()
		{
			IMatch m = new Match();
			m.RemovePlayer(10);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void ResetPlayers_ResetsArr1()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Defender);
			m.AddPlayer(20, PlayerSlot.Challenger);
			m.ResetPlayers();

			Assert.AreEqual(-1, m.DefenderIndex());
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void ResetPlayers_ResetsArr2()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Defender);
			m.AddPlayer(20, PlayerSlot.Challenger);
			m.ResetPlayers();

			Assert.AreEqual(-1, m.ChallengerIndex());
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddWin_AddsAWin()
		{
			int[] score = new int[2] { 0, 1 };
			IMatch m = new Match(3, score, new ushort[2] { 1, 0 }, 0, 0, 0, new List<int>(), 0, 0);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1 + 1 + 1, m.Score[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void AddWin_ThrowsOutOfRange_WithBadInput()
		{
			IMatch m = new Match();
			m.AddWin((PlayerSlot)3);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_WithNotEnoughPlayersInMatch()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_IfMatchIsAlreadyWon()
		{
			IMatch m = new Match();
			m.AddPlayer(10, PlayerSlot.Defender);
			m.AddPlayer(11, PlayerSlot.Challenger);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Defender);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayer_ResetsScore()
		{
			int pIndex = 10;
			IMatch m = new Match();
			m.AddPlayer(pIndex, PlayerSlot.Defender);
			m.AddPlayer(11, PlayerSlot.Challenger);
			m.AddWin(PlayerSlot.Defender);
			m.RemovePlayer(pIndex);

			Assert.AreEqual(0, m.Score[0]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void SubtractWin_Subtracts()
		{
			IMatch m = new Match();
			m.WinsNeeded = 3;
			m.AddPlayer(5);
			m.AddPlayer(6);
			m.AddWin(PlayerSlot.Challenger);
			m.AddWin(PlayerSlot.Challenger);
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(1, m.Score[(int)PlayerSlot.Challenger]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void SubtractWin_ThrowsOutOfRange_WithBadInput()
		{
			IMatch m = new Match();
			m.SubtractWin((PlayerSlot)3);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void SubtractWin_ThrowsException_WhenScoreIsZero()
		{
			IMatch m = new Match();
			m.SubtractWin(PlayerSlot.Challenger);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void ResetScore_ResetsToZero()
		{
			IMatch m = new Match();
			m.WinsNeeded = 2;
			m.AddPlayer(1);
			m.AddPlayer(2);
			m.AddWin(PlayerSlot.Defender);
			m.AddWin(PlayerSlot.Challenger);
			m.ResetScore();

			Assert.AreEqual(0, m.Score[(int)PlayerSlot.Challenger]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
		public void SetMatchNumber_Sets()
		{
			int n = 5;
			IMatch m = new Match();
			m.SetMatchNumber(n);

			Assert.AreEqual(n, m.MatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
		public void SetNextMatchNumber_Sets()
		{
			int n = 5;
			IMatch m = new Match();
			m.SetNextMatchNumber(n);

			Assert.AreEqual(n, m.NextMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
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
		[TestCategory("Match Methods")]
		public void SetNextLoserMatchNumber_Sets()
		{
			int n = 7;
			IMatch m = new Match();
			m.SetNextLoserMatchNumber(n);

			Assert.AreEqual(n, m.NextLoserMatchNumber);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
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
