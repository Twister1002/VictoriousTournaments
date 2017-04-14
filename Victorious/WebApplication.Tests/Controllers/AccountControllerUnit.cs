using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web;
using System.Web.Mvc;
using WebApplication.Controllers;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class AccountControllerUnit
    {
        private class FakeAccountController : AccountController
        {
            public FakeAccountController() : base()
            {

            }

            public void setSession(string name, object value)
            {
                Session[name] = value;
            }
        }

        //[TestMethod]
        //[TestCategory("AccountController")]
        //[TestCategory("Controllers")]
        //public void AccountController_Index_Returns_LoginRedirect()
        //{
        //    // Arrange
        //    AccountController controller = new FakeAccountController();
        //    controller.Session["User.UserId"] = null;

        //    // Act
        //    ViewResult result = controller.Index() as ViewResult;

        //    // Assert
        //    Assert.AreEqual("Omg", result.ViewName);
        //}
    }
}
