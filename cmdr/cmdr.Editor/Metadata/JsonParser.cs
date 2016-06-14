using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cmdr.Editor.Metadata
{
    class JsonParser
    {
        public static Metadata FromJson(string jsonStr)
        {
            var wrapper = JObject.Parse(jsonStr).SelectToken("cmdr_metadata");
            return wrapper.ToObject<Metadata>();
        }

        public static string ToJson(Metadata metadata)
        {
            dynamic wrapper = new { cmdr_metadata = metadata };
            return JsonConvert.SerializeObject(wrapper);
        }
    }
}
