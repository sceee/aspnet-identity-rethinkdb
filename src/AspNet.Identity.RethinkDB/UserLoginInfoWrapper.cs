using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.RethinkDB
{
	/// <summary>
	/// Wrapper class for UserLoginInfo.
	/// Needed because UserLoginInfo cannot be decorated with DataContract and DataMember Attibutes and is sealed.
	/// </summary>
	[DataContract]
	public sealed class UserLoginInfoWrapper
	{
		public readonly UserLoginInfo UserLoginInfo;

		public UserLoginInfoWrapper()
		{
			// Needed for RethinkDB Provider deserialization, values will be overwritten
			UserLoginInfo = new UserLoginInfo(null, null);
		}

		public UserLoginInfoWrapper(string loginProvider, string providerKey)
		{
			UserLoginInfo = new UserLoginInfo(loginProvider, providerKey);
		}

		/// <summary>
		/// Provider for the linked login, i.e. Facebook, Google, etc.
		/// </summary>
		[DataMember]
		public string LoginProvider
		{
			get
			{
				return UserLoginInfo.LoginProvider;
			}
			set
			{
				UserLoginInfo.LoginProvider = value;
			}
		}

		/// <summary>
		/// User specific key for the login provider
		/// </summary>
		[DataMember]
		public string ProviderKey
		{
			get
			{
				return UserLoginInfo.ProviderKey;
			}
			set
			{
				UserLoginInfo.ProviderKey = value;
			}
		}
	}
}
