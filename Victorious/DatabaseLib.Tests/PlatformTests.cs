using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class PlatformTests
    {
        [TestMethod]
        public void Get_All_Platforms()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            List<PlatformModel> platforms = new List<PlatformModel>();
            platforms = db.GetAllPlatforms();
            var result = platforms.Count;

            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Delete_Platform()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            var result = db.DeletePlatform(1);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Add_Platform()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            PlatformModel p = new PlatformModel()
            {
                PlatformName = "Xbox"
            };
            var result = db.AddPlatform(p);

            Assert.AreEqual(DbError.SUCCESS, result);
        }

        [TestMethod]
        public void Update_Platform()
        {
            var db = new DatabaseRepository("VictoriousEntities");

            PlatformModel p = db.GetPlatform(3);
            p.PlatformName = "Update Platform";
            var result = db.UpdatePlatform(p);

            Assert.AreEqual(DbError.SUCCESS, result);
        }
    }
}
