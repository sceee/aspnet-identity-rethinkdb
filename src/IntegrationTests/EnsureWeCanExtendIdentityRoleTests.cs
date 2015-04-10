namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.RethinkDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;
	using RethinkDb;

	[TestFixture]
	public class EnsureWeCanExtendIdentityRoleTests : UserIntegrationTestsBase
	{
		private RoleManager<ExtendedIdentityRole> _Manager;
		private ExtendedIdentityRole _Role;

		public class ExtendedIdentityRole : IdentityRole
		{
			public string ExtendedField { get; set; }
		}

		[SetUp]
		public void BeforeEachTestAfterBase()
		{
			var context = new IdentityContext(DatabaseConnection, DB);
			var roleStore = new RoleStore<ExtendedIdentityRole>(context);
			_Manager = new RoleManager<ExtendedIdentityRole>(roleStore);
			_Role = new ExtendedIdentityRole
			{
				Name = "admin"
			};
		}

		[Test]
		public void Create_ExtendedRoleType_SavesExtraFields()
		{
			_Role.ExtendedField = "extendedField";

			_Manager.Create(_Role);

			var savedRole = DatabaseConnection.Run(IdentityContext.DB.Table<ExtendedIdentityRole>("IdentityRoles")).FirstOrDefault();
			Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
		}

		[Test]
		public void Create_ExtendedRoleType_ReadsExtraFields()
		{
			_Role.ExtendedField = "extendedField";

			_Manager.Create(_Role);

			var savedRole = _Manager.FindById(_Role.Id);
			Expect(savedRole.ExtendedField, Is.EqualTo("extendedField"));
		}
	}
}