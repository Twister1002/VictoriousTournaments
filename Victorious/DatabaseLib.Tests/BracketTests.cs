using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class BracketTests
    {
        [TestMethod]
        public void Add_Bracket()
        {
            var db = new DbInterface();

            var bracket = new BracketModel()
            {
                BracketTitle = "Bracket",
                BracketTypeID = 2,
                Finalized = true,
                TournamentID = db.GetAllTournaments()[0].TournamentID,
                NumberOfGroups = 2
            };

            var result = db.AddBracket(bracket);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Get_Bracket()
        {
            var db = new DbInterface();
            
            var result = db.GetBracket(3);

            Assert.AreEqual("Bracket", result.BracketTitle);
        }

        [TestMethod]
        public void Update_Bracket_No_Cascade()
        {
            var db = new DbInterface();

            BracketModel bracket = db.GetBracket(3);

            bracket.BracketTitle = "new title";

            var result = db.UpdateBracket(bracket);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Delete_Bracket()
        {
            var db = new DbInterface();
        }

    }
}
