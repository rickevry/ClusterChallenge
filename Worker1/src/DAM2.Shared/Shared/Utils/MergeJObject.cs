using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class MergeJObject
    {
        public static JObject NullIfInvalid(JObject o1, JObject o2)
        {
            if(IsNull.Any(o1,o2)) return null;

            JObject result = (JObject) o1.DeepClone();
            result.Merge(o2, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            return result;

        }
    }
}
