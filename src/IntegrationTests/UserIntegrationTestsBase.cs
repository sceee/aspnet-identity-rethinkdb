namespace IntegrationTests
{
	using AspNet.Identity.RethinkDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;
	using RethinkDb;
	using RethinkDb.Configuration;

	public class UserIntegrationTestsBase : AssertionHelper
	{
		private static IConnectionFactory connectionFactory = ConfigurationAssembler.CreateConnectionFactory("testcluster");
		protected IdentityContext IdentityContext;
		protected IConnection DatabaseConnection;
		protected IDatabaseQuery DB;

		[SetUp]
		public void BeforeEachTest()
		{
			DatabaseConnection = connectionFactory.Get();
			DB = Query.Db("UnittestDB");
			IdentityContext = new IdentityContext(DatabaseConnection, DB);

			// Cleanup
			DatabaseConnection.Run(DB.TableDrop("IdentityUsers"));
			DatabaseConnection.Run(DB.TableDrop("IdentityRoles"));
		}

		protected UserManager<IdentityUser> GetUserManager()
		{
			var store = new UserStore<IdentityUser>(IdentityContext);
			return new UserManager<IdentityUser>(store);
		}

		protected RoleManager<IdentityRole> GetRoleManager()
		{
			var store = new RoleStore<IdentityRole>(IdentityContext);
			return new RoleManager<IdentityRole>(store);
		}
	}
}