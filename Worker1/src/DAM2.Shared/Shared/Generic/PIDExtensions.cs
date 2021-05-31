using System.Text.RegularExpressions;
using Proto;


namespace DAM2.Core.Shared.Generic
{
	public class PIDValues
	{
		public static readonly PIDValues Empty = new PIDValues();

		public string Tenant { get; set; }
		public string Name { get; set; }
		public string Eid { get; set; }

		public bool Success => this != Empty;

		public bool Failed => this == Empty;
	}

    public static class PIDExtensions
    {
		
	    private static readonly Regex PidValuesRegex = new Regex(@"\/(?<Name>.*)\/(?<Tenant>.*)\/(?<Eid>.*)\$.*", RegexOptions.Compiled);
        public static PIDValues ExtractIdValues(this PID? pid)
		{
			if (pid == null || string.IsNullOrWhiteSpace(pid.Id))
			{
				return PIDValues.Empty;
			}

			Match pidMatch = PidValuesRegex.Match(pid.Id);
			if (!pidMatch.Success)
			{
				return PIDValues.Empty;
			}

			return new PIDValues
			{
				Tenant = pidMatch.Groups["Tenant"].Value,
				Name = pidMatch.Groups["Name"].Value,
				Eid = pidMatch.Groups["Eid"].Value
			};
			
		}
    }
}
