using System;
namespace Tests
{
	public static class ObjectIdHelpers
	{
		public static Guid? SafeParseGuid(this string id)
		{
			Guid parsed;
			if (Guid.TryParse(id, out parsed))
			{
				return parsed;
			}
			return null;
		}
	}
}