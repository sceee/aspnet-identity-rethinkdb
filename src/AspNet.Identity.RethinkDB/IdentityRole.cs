namespace AspNet.Identity.RethinkDB
{
	using System;
	using Microsoft.AspNet.Identity;

	public class IdentityRole : IRole<string>
	{
		public IdentityRole()
		{
			Id = Guid.NewGuid().ToString("N");
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
		}

		public string Id { get; private set; }

		public string Name { get; set; }
	}
}