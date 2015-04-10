namespace IntegrationTests
{
	using System.Linq;
	using AspNet.Identity.RethinkDB;
	using Microsoft.AspNet.Identity;
	using NUnit.Framework;
	using RethinkDb;
	using RethinkDb.Configuration;

	public class UserIntegrationTestsBase : AssertionHelper
	{
		private const string TEST_DB_NAME = "UnittestDB";
		private static IConnectionFactory connectionFactory = ConfigurationAssembler.CreateConnectionFactory("testcluster");
		protected IdentityContext IdentityContext;
		protected IConnection DatabaseConnection;
		protected IDatabaseQuery DB;

		[SetUp]
		public void BeforeEachTest()
		{
			DatabaseConnection = connectionFactory.Get();
			DB = Query.Db(TEST_DB_NAME);
			// Create DB if it does not exist
			if (!DatabaseConnection.Run(Query.DbList()).Contains(TEST_DB_NAME))
				DatabaseConnection.Run(Query.DbCreate(TEST_DB_NAME));

			IdentityContext = new IdentityContext(DatabaseConnection, DB);

			// Cleanup if needed
			if (DatabaseConnection.Run(DB.TableList()).Contains("IdentityUsers"))
				DatabaseConnection.Run(DB.TableDrop("IdentityUsers"));

			if (DatabaseConnection.Run(DB.TableList()).Contains("IdentityRoles"))
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