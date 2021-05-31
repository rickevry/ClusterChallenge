using Newtonsoft.Json.Linq;
using System;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class DeepJObject
    {

        public static JToken GetValue(JObject jobject, string path)
        {
            try
            {
                if (jobject != null && !string.IsNullOrEmpty(path))
                {
                    string[] parts = path.Split(".");
                    return getValue(jobject, parts);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return "";
        }

        public static void SetValue(JObject jobject, string path, object value)
        {
            try
            {
                if (jobject != null && !string.IsNullOrEmpty(path))
                {
                    string[] parts = path.Split(".");
                    setValue(jobject, parts, value);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static JToken getValue(JObject jobject, string[] path)
        {
            string key = path[0];
            if (path.Length == 1)
            {
                return jobject[key];
            }
            else
            {
                JObject child = (JObject)jobject[key];
                if (child == null)
                {
                    child = new JObject();
                    jobject[key] = child;
                }
                return getValue(child, path[1..]);
            }
        }

        static void setValue(JObject jobject, string[] path, object value)
        {
            string key = path[0];
            if (path.Length == 1)
            {
                if (value is JArray)
                {
                    jobject[key] = (JArray)value;
                }
                else
                {
                    if (value.ToString() == "undefined")
                    {
                        jobject.Remove(key);
                    } else
                    {
                        jobject[key] = new JValue(value);
                    }
                }
            }
            else
            {
                JObject child = (JObject)jobject[key];
                if (child == null)
                {
                    child = new JObject();
                    jobject[key] = child;
                }
                setValue(child, path[1..], value);
            }
        }
    }


}
