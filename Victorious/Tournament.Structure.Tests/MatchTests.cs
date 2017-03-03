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
			int pIndex1 = 10;
			int pIndex2 = 20;
			IMatch m = new Match();
			m.AddPlayer(pIndex1, 0);
			m.AddPlayer(pIndex2, 1);

			Assert.AreEqual(pIndex1, m.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPlayer_AddsSecondPlayer()
		{
			int pIndex2 = 20;
			IMatch m = new Match();
			m.AddPlayer(pIndex2, 1);

			Assert.AreEqual(pIndex2, m.PlayerIndexes[1]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(IndexOutOfRangeException))]
		public void AddPlayer_ThrowsException_WithBadIndexParam()
		{
			IMatch m = new Match();
			m.AddPlayer(1, 4);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(Exception))]
		public void AddPlayer_ThrowsException_WhenAddingPlayerToSameSpot()
		{
			IMatch m = new Match();
			m.AddPlayer(10, 0);
			m.AddPlayer(20, 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(DuplicateObjectException))]
		public void AddPlayer_ThrowsDuplicate_WhenAddingSamePlayerTwice()
		{
			IMatch m = new Match();
			m.AddPlayer(10, 0);
			m.AddPlayer(10, 1);

			Assert.AreEqual(1, 2);
		}
		
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayer_Removes()
		{
			int pIndex = 10;
			IMatch m = new Match();
			m.AddPlayer(pIndex, 0);
			m.RemovePlayer(pIndex);

			Assert.AreEqual(-1, m.PlayerIndexes[0]);
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
		public void RemovePlayers_ResetsArr1()
		{
			IMatch m = new Match();
			m.AddPlayer(10, 0);
			m.AddPlayer(20, 1);
			m.RemovePlayers();

			Assert.AreEqual(-1, m.PlayerIndexes[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void RemovePlayers_ResetsArr2()
		{
			IMatch m = new Match();
			m.AddPlayer(10, 0);
			m.AddPlayer(20, 1);
			m.RemovePlayers();

			Assert.AreEqual(-1, m.PlayerIndexes[1]);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddWin_AddsAWin()
		{
			int[] score = new int[2] { 0, 1 };
			IMatch m = new Match(3, score, new ushort[2] { 1, 0 }, 0, 0, 0, new List<int>(), 0, 0);
			m.AddWin(0);
			m.AddWin(0);

			Assert.AreEqual(1 + 1 + 1, m.Score[0]);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_WithNoPlayersInMatch()
		{
			IMatch m = new Match();
			m.AddWin(0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(InactiveMatchException))]
		public void AddWin_ThrowsInactiveMatch_IfMatchIsAlreadyWon()
		{
			IMatch m = new Match();
			m.AddPlayer(10, 0);
			m.AddPlayer(11, 1);
			m.AddWin(0);
			m.AddWin(0);

			Assert.AreEqual(1, 2);
		}

		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		public void AddPrevMatchIndex_Adds()
		{
			int i = 14;
			IMatch m = new Match();
			m.AddPrevMatchIndex(i);

			Assert.IsTrue(m.PrevMatchIndexes.Contains(i));
		}
		[TestMethod]
		[TestCategory("Match")]
		[TestCategory("Match Methods")]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void AddPrevMatchIndex_ThrowsOutOfRange_AfterMoreThanTwoCalls()
		{
			IMatch m = new Match();
			m.AddPrevMatchIndex(0);
			m.AddPrevMatchIndex(1);
			m.AddPrevMatchIndex(2);

			Assert.AreEqual(1, 2);
		}
	}
}
