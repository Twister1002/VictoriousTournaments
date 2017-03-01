using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Tournament.Structure.Tests
{
	[TestClass]
	public class UserTests
	{
		[TestMethod]
		[TestCategory("User")]
		[TestCategory("User Constructor")]
		public void UserDefaultCtor_Constructs()
		{
			IPlayer u = new User();

			Assert.IsInstanceOfType(u, typeof(User));
		}
		[TestMethod]
		[TestCategory("User")]
		[TestCategory("User Constructor")]
		public void UserOverloadedCtor_SavesData()
		{
			string last = "Johnson";
			IPlayer u = new User(1, "user", "first", last, "email");

			Assert.AreEqual(last, (u as User).Lastname);
		}
		[TestMethod]
		[TestCategory("User")]
		[TestCategory("User Constructor")]
		[ExpectedException(typeof(NullReferenceException))]
		public void UserOverloadedCtor_ThrowsNullRef_OnNullParams()
		{
			IPlayer u = new User(1, null, null, null, null);

			Assert.AreEqual(1, 2);
		}
		[TestMethod]
		[TestCategory("User")]
		[TestCategory("User Constructor")]
		[TestCategory("Models")]
		[ExpectedException(typeof(NullReferenceException))]
		public void UserModelCtor_ThrowsNullRef_OnNullParams()
		{
			IPlayer u = new User(null);

			Assert.AreEqual(1, 2);
		}
	}
}
