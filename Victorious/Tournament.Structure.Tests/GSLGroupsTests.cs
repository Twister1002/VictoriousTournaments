using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;

using DatabaseLib;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class GSLGroupsTests
	{
		#region Bracket Creation
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		public void GSLCtor_Constructs()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.IsInstanceOfType(b, typeof(GSLGroups));
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GSLCtor_ThrowsNull_WithNullParameter()
		{
			IBracket b = new GSLGroups(null, 2);
			Assert.AreEqual(1, 2);
		}
#if false
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL Ctor")]
		[ExpectedException(typeof(BracketException))]
		public void GSLCtor_ThrowsBracketExcep_WithNonStandardPlayerCount()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 6; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);
		}
#endif

		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		[ExpectedException(typeof(ScoreException))]
		public void GSLCreateBracket_ThrowsScoreExcep_WithNegativeGamesPerMatch()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2, 0);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_CreatesTenMatches_ForTwoGroupsOfFour()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(10, b.NumberOfMatches);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_CorrectlyAssignsNumberOfRounds()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(2, b.NumberOfRounds);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_GroupStageStoresAllEightPlayers()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			Assert.AreEqual(pList.Count, b.NumberOfPlayers());
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL CreateBracket")]
		public void GSLCreateBracket_PutsDifferentPlayersInEachGroup()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			List<int> firstGroupIds = new List<int>();
			foreach (IMatch match in (b as IGroupStage).GetRound(1, 1))
			{
				firstGroupIds.AddRange(match.Players.Select(p => p.Id));
			}
			List<int> secondGroupIds = new List<int>();
			foreach (IMatch match in (b as IGroupStage).GetRound(2, 1))
			{
				secondGroupIds.AddRange(match.Players.Select(p => p.Id));
			}
			Assert.AreEqual(0, firstGroupIds.Intersect(secondGroupIds).ToList().Count);
		}
		#endregion

		#region Bracket Progression
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_UpperBracketOnlyAddsOnePlayerToRankings()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			int group2rounds = (b as IGroupStage).NumberOfRoundsInGroup(2);
			int winnerId = 0;
			for (int r = 1; r <= group2rounds; ++r)
			{
				List<IMatch> round = (b as IGroupStage).GetRound(2, r);
				for (int m = 0; m < round.Count; ++m)
				{
					b.AddGame(round[m].MatchNumber, 1, 0, PlayerSlot.Defender);
					winnerId = round[m].Players[(int)PlayerSlot.Defender].Id;
				}
			}

			Assert.AreEqual(1, b.Rankings.Count);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_GivesRank1ToUpperBracketWinners()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 12; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i + 1);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 3);

			for (int r = 1; r <= b.NumberOfRounds; ++r)
			{
				List<IMatch> round = b.GetRound(r);
				foreach (IMatch match in round)
				{
					b.AddGame(match.MatchNumber, 1, 0, PlayerSlot.Defender);
				}
			}

			Assert.IsTrue(b.Rankings.All(r => r.Rank == 1));
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_SetsGroupStageAsFinishedWhenAllMatchesAreDone()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.IsTrue(b.IsFinished);
		}
		[TestMethod]
		[TestCategory("GSLGroups")]
		[TestCategory("GSL AddGame")]
		public void GSLAddGame_AddsEveryPlayerToRankingsWhenBracketIsFinished()
		{
			List<IPlayer> pList = new List<IPlayer>();
			for (int i = 1; i <= 8; ++i)
			{
				Mock<IPlayer> moq = new Mock<IPlayer>();
				moq.Setup(p => p.Id).Returns(i);
				pList.Add(moq.Object);
			}
			IBracket b = new GSLGroups(pList, 2);

			for (int n = 1; n <= b.NumberOfMatches; ++n)
			{
				b.AddGame(n, 1, 0, PlayerSlot.Defender);
			}
			Assert.AreEqual(b.NumberOfPlayers(), b.Rankings.Count);
		}
		#endregion
	}
}
