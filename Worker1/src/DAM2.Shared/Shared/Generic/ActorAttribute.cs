using System;

namespace DAM2.Core.Actors.Shared.Generic
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ActorAttribute : Attribute
    {
		public ActorAttribute(string kind)
	    {
		    this.Kind = kind;
	    }
	    public string Kind { get; set; }
    }
}
