using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseLib.Services;

namespace DatabaseLib.Tests
{
    [TestClass]
    public class TypeServiceTests
    {
        IUnitOfWork unitOfWork;
        TypeService service;

        [TestInitialize]
        public void Initialize()
        {
            unitOfWork = new UnitOfWork();
            service = new TypeService(unitOfWork);
        }

        #region BracketTypes

        [TestMethod]
        [TestCategory("Type Service")]
        public void UpdateBracketType_Save()
        {
            BracketTypeModel bracketType = service.GetBracketType(1);
            bracketType.IsActive = false;
            service.UpdateBracketType(bracketType);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion


        #region Platforms

        [TestMethod]
        [TestCategory("Type Service")]
        public void AddPlatform_Save()
        {
            PlatformModel p = new PlatformModel()
            {
                PlatformName = "Xbox 720"
            };
            service.AddPlatform(p);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Type Service")]
        public void GetPlatform()
        {
            PlatformModel p = service.GetPlatform(1002);

            Assert.AreEqual("Xbox 720", p.PlatformName);
        }

        [TestMethod]
        [TestCategory("Type Service")]
        public void UpdatePlatform_Save()
        {
            PlatformModel p = service.GetPlatform(1002);
            p.PlatformName = "Xbox 1080";
            service.UpdatePlatform(p);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        [TestCategory("Type Service")]
        public void DeletePlatform_Save()
        {
            service.DeletePlatform(1002);
            var result = unitOfWork.Save();

            Assert.AreEqual(true, result);
        }

        #endregion
    }
}
