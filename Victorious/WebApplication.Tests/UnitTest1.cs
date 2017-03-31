using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WebApplication.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AccountController_Logout_Destroys_All_Sessions()
        {
            Session["Hello"] = "Yes";
            Session["What the fuck"] = "Helllo";

        }
    }
}
