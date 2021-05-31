using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class JTokenToObject
    {
        public static object ConvertOrNull(JValue t)
        {
            if (t!=null)
            {
                return t.Value;
            }
            return null;
        }

        public static object ConvertOrNull(JArray t)
        {
            if (t != null)
            {
                return t;
            }
            return null;
        }

    }
}
