using System.Runtime.Serialization;
using System.Security.Claims;

namespace AspNet.Identity.RethinkDB
{
	[DataContract]
	public class IdentityUserClaim
	{
		public IdentityUserClaim()
		{
		}

		public IdentityUserClaim(Claim claim)
		{
			Type = claim.Type;
			Value = claim.Value;
		}

		[DataMember]
		public string Type { get; set; }

		[DataMember]
		public string Value { get; set; }

		public Claim ToSecurityClaim()
		{
			return new Claim(Type, Value);
		}
	}
}