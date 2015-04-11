using System;
using System.Runtime.Serialization;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RethinkDB
{
	[DataContract]
	public class IdentityRole : IRole<string>
	{
		public IdentityRole()
		{
			Id = Guid.NewGuid().ToString("N");
		}

		public IdentityRole(string roleName)
			: this()
		{
			Name = roleName;
		}

		// TODO: Make private set. But: if that is private set, an Exception in RethinkDB-driver DataContractDatumConverterFactory occurs.
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}