using Newtonsoft.Json.Linq;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class UpdateJObject
    {

        private static void SetValue(JObject job, string key, JValue value)
        {
            DeepJObject.SetValue(job, key, JTokenToObject.ConvertOrNull(value));
        }

        private static void SetValue(JObject job, string key, JArray value)
        {
            DeepJObject.SetValue(job, key, JTokenToObject.ConvertOrNull(value));
        }

        public static void FromJArray(JObject job, JArray jArray)
        {
            if (jArray != null)
            {
                foreach (JObject updateObject in jArray)
                {
                    JToken key = updateObject.GetValue("key");
                    JToken value = updateObject.GetValue("value");

                    if (key != null && key.ToString() != "")
                    {
                        var keyStr = key.ToString();
                        if (keyStr.StartsWith("props."))
                        {
                            keyStr = keyStr.Substring("props.".Length);
                        }

                        if (value is JValue)
                        {
                            SetValue(job, keyStr, (JValue)value);
                        }
                        else if (value is JArray)
                        {
                            SetValue(job, keyStr, (JArray)value);
                        }
                    }
                }
            }
        }
    }
}
