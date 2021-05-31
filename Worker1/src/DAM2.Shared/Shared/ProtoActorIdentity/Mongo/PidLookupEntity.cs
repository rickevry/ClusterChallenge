using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace DAM2.Shared.ProtoActorIdentity.Mongo
{
	public class PidLookupEntity
	{
		[BsonId]
		public string Key { get; set; } = default!;
		public string Identity { get; set; } = default!;
		public string? UniqueIdentity { get; set; } = default!;
		public string Kind { get; set; } = default!;
		public string? Address { get; set; }
		public string? MemberId { get; set; }
		public string? LockedBy { get; set; }
		public int Revision { get; set; }
	}
}
