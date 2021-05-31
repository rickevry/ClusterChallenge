using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class StringifyJArray
    {
        public static string ToJsonOrNUll(JArray o)
        {
            if (o!=null)
            {
                return o.ToString(Newtonsoft.Json.Formatting.None);
            }
            return null;
        }

    }
}
