using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Influxdb.Infrastructure
{
    public static class DynamicObjectExtensions
    {
        public static Dictionary<string, object> ToDictionary(this ExpandoObject dyn)
        {
            var dic = dyn.ToDictionary(x => x.Key, x => x.Value);
            return dic;
        }

        public static T ToObject<T>(this ExpandoObject dyn) where T : class
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dyn);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static List<T> ToObject<T>(this List<ExpandoObject> lstDyn) where T : class
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstDyn);
            var lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
            return lst;
        }

    }
}
