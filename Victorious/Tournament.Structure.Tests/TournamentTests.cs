using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Tournament.Structure;

namespace Tournament.Structure.Tests
{
	[TestClass]
	public class TournamentTests
	{
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
	}
}
