namespace AspNet.Identity.RethinkDB
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	//using global::MongoDB.Bson;
	//using global::MongoDB.Driver.Builders;
	//using global::MongoDB.Driver.Linq;
	using Microsoft.AspNet.Identity;
	using RethinkDb.Driver.Ast;

	/// <summary>
	///     Note: Deleting and updating do not modify the roles stored on a user document. If you desire this dynamic
	///     capability, override the appropriate operations on RoleStore as desired for your application. For example you could
	///     perform a document modification on the users collection before a delete or a rename.
	/// </summary>
	/// <typeparam name="TRole"></typeparam>
	public class RoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
		where TRole : IdentityRole
	{
		private readonly IdentityContext _Context;
		//private readonly ITableQuery<TRole> TableRoles;
		private readonly Table TableRoles;

		public RoleStore(IdentityContext context)
		{
			_Context = context;
			TableRoles = _Context.DB.Table("IdentityRoles"); //. Table<TRole>("IdentityRoles");
		}

		public virtual IQueryable<TRole> Roles
		{
			// TODO: Performance?!
			get
			{
				//return _Context.Connection.Run(TableRoles).AsQueryable<TRole>();
				IQueryable<TRole> result = TableRoles.Run<IQueryable<TRole>>(_Context.Connection);
				return result;
			}
		}

		public virtual void Dispose()
		{
			// no need to dispose of anything, mongodb handles connection pooling automatically
		}

		public virtual Task CreateAsync(TRole role)
		{
			return Task.Run(() => TableRoles.Insert(role).Run(_Context.Connection)); //  _Context.Connection.Run(TableRoles.Insert(role)));
		}

		public virtual Task UpdateAsync(TRole role)
		{
			return Task.Run(() => TableRoles.Update(role).Run(_Context.Connection)); //_Context.Connection.Run(TableRoles.Insert(role, Conflict.Replace)));
		}

		public virtual Task DeleteAsync(TRole role)
		{
			return Task.Run(() => TableRoles.Get(role.Id).Delete()); // _Context.Connection.Run(TableRoles.Get(role.Id).Delete())); // Select(r => r.Id == role.Id).Delete()));
		}

		public virtual Task<TRole> FindByIdAsync(string roleId)
		{
			return Task.Run(() => TableRoles.Get(roleId).RunAtom<TRole>(_Context.Connection)); // _Context.Connection.Run(TableRoles.Filter(r => r.Id == roleId)).FirstOrDefault()); // FindOneByIdAs<TRole>(ObjectId.Parse(roleId)));
		}

		public virtual Task<TRole> FindByNameAsync(string roleName)
		{
			//var queryByName = Query.Filter<TRole>(r,  Query<TRole>.EQ(r => r.Name, roleName);
			return Task.Run(() => TableRoles.Filter(role => role["Name"] == roleName).RunAtom<TRole>(_Context.Connection));//  _Context.Connection.Run(TableRoles.Filter<TRole>(r => r.Name == roleName)).FirstOrDefault());
		}
	}
}