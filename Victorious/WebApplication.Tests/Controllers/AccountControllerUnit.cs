using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication.Controllers;

namespace WebApplication.Tests.Controllers
{
    [TestClass]
    public class AccountControllerUnit
    {
        [TestMethod]
        public void AccountController_Logout_DestroysAllSessions()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act
            

            // Assert
        }

        [TestMethod]
        public void AccountController_Index_Returns_LoginRedirect()
        {
            // Arrange
            AccountController controller = new AccountController();

            // Act


            // Assert
        }
    }
}
