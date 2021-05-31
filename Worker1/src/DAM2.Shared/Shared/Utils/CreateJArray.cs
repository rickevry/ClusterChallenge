using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAM2.Core.Actors.Shared.Utils
{
    public class CreateJArray
    {

        public static JArray FromListOfJObject(List<JObject> list)
        {
            JArray array = new JArray();
            if (list!=null)
            {
                foreach (object obj in list) {
                    if (obj is JObject)
                    {
                        array.Add(obj);
                    }
                }
            }
            return array;
        }
    }
}
