AspNet.Identity.RethinkDB
=====================

This project is currently in development stadium and not yet ready for production use.

A rethinkdb provider for the ASP.NET Identity framework.
This provider uses rethinkdb-net (available as NuGet package or https://github.com/mfenniak/rethinkdb-net).

## Usage

You will need to create a database connection and hand that over to the IdentityContext constructor.
The following is a example implementation that uses the provider for the Asp.Net Identity Framework.

In your Web.config, define the database connection as normal when using rethinkdb-net:
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
		<configSections>
				<section name="rethinkdb" type="RethinkDb.Configuration.RethinkDbClientSection, RethinkDb"/>
		</configSections>
		<rethinkdb>
				<clusters>
						<cluster name="dbcluster">
								<defaultLogger enabled="true" category="Warning"/>
								<connectionPool enabled="true"/>
								<networkErrorHandling enabled="true" />
								<endpoints>
										<endpoint address="127.0.0.1" port="28015"/>
								</endpoints>
						</cluster>
				</clusters>
		</rethinkdb>
		...
		other configuration
		...
</configuration>
```

```C#
// Create class for ApplicationIdentityContext that derives from IdentityContext

public class ApplicationIdentityContext : IdentityContext, IDisposable
{
	private const string DB_NAME = "myDB";

	public ApplicationIdentityContext(IConnection conn, IDatabaseQuery db)
		: base(conn, db)
	{
	}

	public static ApplicationIdentityContext Create()
	{
		// Load connection data from Web.config
		IConnectionFactory connectionFactory = ConfigurationAssembler.CreateConnectionFactory("dbcluster");
		// Get the connection to the database server(s)
		IConnection databaseConnection = connectionFactory.Get();
		// Get an object to use the database
		IDatabaseQuery DB = Query.Db(DB_NAME);

#if DEBUG
		// Create DB if it does not exist
		if (!databaseConnection.Run(Query.DbList()).Contains(DB_NAME))
			databaseConnection.Run(Query.DbCreate(DB_NAME));
#endif
		return new ApplicationIdentityContext(databaseConnection, DB);
	}

	public void Dispose()
	{
	}
}

// Configure RoleManager for the application
```C#
public class ApplicationRoleManager : RoleManager<IdentityRole>
{
	public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
		: base(roleStore)
	{
	}

	public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
	{
		var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationIdentityContext>()));

		return manager;
	}
}
```

Configure UserManager for the application
```C#
public class ApplicationUserManager : UserManager<ApplicationUser>
{
	public ApplicationUserManager(IUserStore<ApplicationUser> store)
		: base(store)
	{
	}

	public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
	{
		var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationIdentityContext>()));
		// Configure validation logic for usernames
		manager.UserValidator = new UserValidator<ApplicationUser>(manager)
		{
			AllowOnlyAlphanumericUserNames = false,
			RequireUniqueEmail = true
		};
		// Configure validation logic for passwords
		manager.PasswordValidator = new PasswordValidator
		{
			RequiredLength = 4,
			RequireNonLetterOrDigit = false,
			RequireDigit = false,
			RequireLowercase = false,
			RequireUppercase = false,
		};
		// Configure user lockout defaults
		manager.UserLockoutEnabledByDefault = true;
		manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
		manager.MaxFailedAccessAttemptsBeforeLockout = 5;
		// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
		// You can write your own provider and plug in here.
		manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
		{
			MessageFormat = "Your security code is: {0}"
		});
		manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
		{
			Subject = "SecurityCode",
			BodyFormat = "Your security code is {0}"
		});
		manager.EmailService = new EmailService();
		manager.SmsService = new SmsService();
		var dataProtectionProvider = options.DataProtectionProvider;
		if (dataProtectionProvider != null)
		{
			manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
		}
		return manager;
	}

	/// <summary>
	/// Method to add user to multiple roles
	/// </summary>
	/// <param name="userId">user id</param>
	/// <param name="roles">list of role names</param>
	/// <returns></returns>
	public virtual async Task<IdentityResult> AddUserToRolesAsync(string userId, IList<string> roles)
	{
		var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

		var user = await FindByIdAsync(userId).ConfigureAwait(false);
		if (user == null)
		{
			throw new InvalidOperationException("Invalid user Id");
		}

		var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
		// Add user to each role using UserRoleStore
		foreach (var role in roles.Where(role => !userRoles.Contains(role)))
		{
			await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
		}

		// Call update once when all roles are added
		return await UpdateAsync(user).ConfigureAwait(false);
	}

	/// <summary>
	/// Remove user from multiple roles
	/// </summary>
	/// <param name="userId">user id</param>
	/// <param name="roles">list of role names</param>
	/// <returns></returns>
	public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(string userId, IList<string> roles)
	{
		var userRoleStore = (IUserRoleStore<ApplicationUser, string>)Store;

		var user = await FindByIdAsync(userId).ConfigureAwait(false);
		if (user == null)
		{
			throw new InvalidOperationException("Invalid user Id");
		}

		var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);
		// Remove user to each role using UserRoleStore
		foreach (var role in roles.Where(userRoles.Contains))
		{
			await userRoleStore.RemoveFromRoleAsync(user, role).ConfigureAwait(false);
		}

		// Call update once when all roles are removed
		return await UpdateAsync(user).ConfigureAwait(false);
	}
}

// In your ASP.net application, you should have a Startup.Auth.cs file containing a partial class Startup:

public partial class Startup
{
		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
	// Configure the db context, user manager and role manager to use a single instance per request
	app.CreatePerOwinContext(ApplicationIdentityContext.Create);
	app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
	app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);

	// Enable the application to use a cookie to store information for the signed in user
	// and to use a cookie to temporarily store information about a user logging in with a third party login provider
	// Configure the sign in cookie
	app.UseCookieAuthentication(new CookieAuthenticationOptions
	{
		AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
		LoginPath = new PathString("/Account/Login"),
		Provider = new CookieAuthenticationProvider
		{
			// Enables the application to validate the security stamp when the user logs in.
			// This is a security feature which is used when you change a password or add an external login to your account.  
			OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
				validateInterval: TimeSpan.FromMinutes(30),
				regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
		}
	});

	app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

	// Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
	app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

	// Enables the application to remember the second login verification factor such as phone or email.
	// Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
	// This is similar to the RememberMe option when you log in.
	app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

				// Uncomment the following lines to enable logging in with third party login providers
				//app.UseMicrosoftAccountAuthentication(
				//    clientId: "",
				//    clientSecret: "");

				//app.UseTwitterAuthentication(
				//   consumerKey: "",
				//   consumerSecret: "");

				//app.UseFacebookAuthentication(
				//   appId: "",
				//   appSecret: "");

				//app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
				//{
					//ClientId = "clientid",
					//ClientSecret = "clientsecret"
				//});
		}
}
```
#NOTIMPLEMENTEDYET
	// at some point in application startup it would be good to ensure unique indexes on user and role names exist, these used to be a part of IdentityContext, but that caused issues for people that didn't want the indexes created at the time the IdentityContext is created. They're now just part of the static IndexChecks:

	IndexChecks.EnsureUniqueIndexOnUserName(users);
	IndexChecks.EnsureUniqueIndexOnEmail(users);

	IndexChecks.EnsureUniqueIndexOnRoleName(roles);



I will provide a complete sample in the future which may be based on the [Microsoft ASP.NET Identity Samples](http://www.nuget.org/packages/Microsoft.AspNet.Identity.Samples).

## Installation
TODO
via nuget:

	Install-Package AspNet.Identity.RethinkDB

## Building and Testing
TODO
I'm using the albacore project with rake.

To build:

	rake msbuild

To test:

	rake tests
	rake integration_tests

To package:

	rake package

## Documentation
TODO

## License
The project is licensed under the MIT License (MIT). It is based on the code for the mongodb provider for Asp.Net Identity Framework by g0t4 ( https://github.com/g0t4/aspnet-identity-mongo ) which is also licensed under the MIT License. Thanks for this code.
