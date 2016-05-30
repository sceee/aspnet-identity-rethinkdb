namespace AspNet.Identity.RethinkDB
{
	using System;
	using global::RethinkDb;
	using RethinkDb.Driver.Ast;
	using RethinkDb.Driver.Net;

	public class IdentityContext
	{
		public IConnection Connection { get; private set; }
		public Db DB { get; private set; }
		//public ITableQuery<TUser> Users { get; set; }
		//public ITableQuery<TRole> Roles { get; set; }

		/*
		 * var client = new MongoClient("mongodb://traggerapp:NuHoepEF6hK8ywXz@ds053479.mongolab.com:53479/traggr");
			var database = client.GetServer().GetDatabase("traggr");
			var users = database.GetCollection<IdentityUser>("users");
			var roles = database.GetCollection<IdentityRole>("roles");
			return new ApplicationIdentityContext(users, roles);
		 */

		//public IdentityContext()
		//{
		//}

		public IdentityContext(IConnection conn, Db db)
		{
			Connection = conn;
			DB = db;
		}

		//public IdentityContext(IConnection conn, ITableQuery<TUser> users, ITableQuery<TRole> roles)
		//	: this(conn, users)
		//{
		//	Roles = roles;
		//}

		//[Obsolete("Use IndexChecks.EnsureUniqueIndexOnEmail")]
		//public void EnsureUniqueIndexOnEmail()
		//{
		//	IndexChecks.EnsureUniqueIndexOnEmail(Users);
		//}
	}
}
