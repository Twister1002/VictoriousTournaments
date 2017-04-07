using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataLib.Tests
{
    [TestClass]
    public class ConcurrencyTests
    {
        [TestMethod]
        public void Test_Tournament_Concurrency()
        {
            var db = new DatabaseInterface();

            var t1 = db.GetTournamentById(1);
            var t2 = db.GetTournamentById(1);

            t1.Title = "Update";
            //db.UpdateTournament(t1);

            t2.Title = "Me Is Error";
            var result = db.UpdateTournament(t2);

            Assert.AreEqual(DbError.CONCURRENCY_ERROR, result);
        }
    }
}
