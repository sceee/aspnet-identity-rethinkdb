using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AspNet.Identity.RethinkDB;
using Microsoft.AspNet.Identity;

namespace SampleApplication.Models
{
	[DataContract]
	public class ApplicationUser : IdentityUser
	{
		[DataMember]
		public string Firstname { get; set; }
		[DataMember]
		public string Lastname { get; set; }
		[DataMember]
		public byte[] ProfileImage24x24 { get; set; }

		public string Initials
		{
			get
			{
				if (string.IsNullOrEmpty(Firstname) || string.IsNullOrEmpty(Lastname))
					return null;
				else
					return Firstname.Substring(0, 1) + Lastname.Substring(0, 1);
			}
		}

		public string Fullname
		{
			get
			{
				return string.Concat(Firstname, " ", Lastname);
			}
		}

		[DataMember]
		public DateTime RegisteredAt { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}
}