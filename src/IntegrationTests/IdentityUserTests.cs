namespace IntegrationTests
{
	using System;
	using AspNet.Identity.RethinkDB;
	using NUnit.Framework;
	using Tests;
	using RethinkDb;

	[TestFixture]
	public class IdentityUserTests : UserIntegrationTestsBase
	{
		[Test]
		public void Insert_NoId_SetsId()
		{
			var user = new IdentityUser();
			user.SetId(null);

			DatabaseConnection.Run(IdentityContext.DB.Table<IdentityUser>("IdentityUsers").Insert(user));
			//Users.Insert(user);

			Expect(user.Id, Is.Not.Null);
			var parsed = user.Id.SafeParseGuid();
			Expect(parsed, Is.Not.Null);
			Expect(parsed, Is.Not.EqualTo(Guid.Empty));
		}
	}
}