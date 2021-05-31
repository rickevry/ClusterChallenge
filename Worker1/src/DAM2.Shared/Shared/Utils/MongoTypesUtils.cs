using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM2.Core.Actors.Shared.Utils
{
    public static class MongoTypesUtils
    {
		public static string GetValue(BsonValue term, string key)
		{
			try
            {
				BsonValue value = term?.AsBsonDocument?.GetValue(key);
				if (value != null && !value.IsBsonNull && value.IsString)
				{
					return value.AsString;
				}
			}
			catch
            {
				// empty string is returned
            }

			return string.Empty;
		}
	}
}
