using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeUpMessangerServer.Modules
{
    public class JsonName
    {
        public const string ID = "ID";
        public const string Password = "Password";
    }

    public class JsonHelper
    {
        public JsonHelper()
        {

        }

        public Dictionary<string, string> GetLoginInfo(string data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            JsonParser jsonParser = new JsonParser(data);

            result.Add(JsonName.ID, jsonParser.GetStringValue(JsonName.ID));
            result.Add(JsonName.Password, jsonParser.GetStringValue(JsonName.Password));

            return result;
        }
    }
}
