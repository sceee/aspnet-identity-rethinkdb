using System.Linq;
using System.Runtime.Serialization;
using AspNet.Identity.RethinkDB;
using Microsoft.AspNet.Identity;
using NUnit.Framework;
using RethinkDb;

namespace IntegrationTests
{
	[TestFixture]
	public class EnsureWeCanExtendIdentityUserTests : UserIntegrationTestsBase
	{
		private UserManager<ExtendedIdentityUser> _Manager;
		private ExtendedIdentityUser _User;

		[DataContract]
		public class ExtendedIdentityUser : IdentityUser
		{
			[DataMember]
			public string ExtendedField { get; set; }
		}

		[SetUp]
		public void BeforeEachTestAfterBase()
		{
			var context = new IdentityContext(DatabaseConnection, DB);
			var userStore = new UserStore<ExtendedIdentityUser>(context);
			_Manager = new UserManager<ExtendedIdentityUser>(userStore);
			_User = new ExtendedIdentityUser
			{
				UserName = "bob"
			};
		}

		[Test]
		public void Create_ExtendedUserType_SavesExtraFields()
		{
			_User.ExtendedField = "extendedField";

			_Manager.Create(_User);

			var savedUser = DatabaseConnection.Run(IdentityContext.DB.Table<ExtendedIdentityUser>("IdentityUsers")).FirstOrDefault();
			Expect(savedUser.ExtendedField, Is.EqualTo("extendedField"));
		}

		[Test]
		public void Create_ExtendedUserType_ReadsExtraFields()
		{
			_User.ExtendedField = "extendedField";

			_Manager.Create(_User);

			var savedUser = _Manager.FindById(_User.Id);
			Expect(savedUser.ExtendedField, Is.EqualTo("extendedField"));
		}
	}
}