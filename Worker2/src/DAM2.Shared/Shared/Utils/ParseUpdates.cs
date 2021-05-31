using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class ParseUpdates
    {

        public static JArray Safe(string updates)
        {
            if (!string.IsNullOrEmpty(updates))
            {
                JArray jArray = JArray.Parse(updates);
                return jArray;
            }
            return new JArray();
        }
    }
}
