using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class StringifyJObject
    {
        public static string ToJsonOrNUll(JObject o)
        {
            if (o!=null)
            {
                return o.ToString(Newtonsoft.Json.Formatting.None);
            }
            return null;
        }

    }
}
