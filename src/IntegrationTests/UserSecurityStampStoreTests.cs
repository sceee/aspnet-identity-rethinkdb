namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.RethinkDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;
	using RethinkDb;

	[TestFixture]
	public class UserSecurityStampStoreTests : UserIntegrationTestsBase
	{
		[Test]
		public void Create_NewUser_HasSecurityStamp()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};

			manager.Create(user);

			var savedUser = DatabaseConnection.Run(IdentityContext.DB.Table<IdentityUser>("IdentityUsers")).FirstOrDefault();
			Expect(savedUser.SecurityStamp, Is.Not.Null);
		}

		[Test]
		public void GetSecurityStamp_NewUser_ReturnsStamp()
		{
			var manager = GetUserManager();
			var user = new IdentityUser {UserName = "bob"};
			manager.Create(user);

			var stamp = manager.GetSecurityStamp(user.Id);

			Expect(stamp, Is.Not.Null);
		}
	}
}