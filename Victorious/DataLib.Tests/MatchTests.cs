using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLib.Tests
{
    [TestClass]
    public class MatchTests
    {
        DatabaseInterface db = null;
        BracketModel bracket = null;
        MatchModel match = null;

        [ClassInitialize]
        public void TestSetup()
        {
            db = new DatabaseInterface();
            bracket = db.GetBracketByID(1);
        }


        [TestMethod]
        public void Create_Match()
        {
            match = new MatchModel()
            {
                Challenger = db.GetUserById(1),
                Defender = db.GetUserById(2),

            };

        }
    }
}
