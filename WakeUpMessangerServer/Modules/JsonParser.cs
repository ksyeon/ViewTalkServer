using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Json;

namespace WakeUpMessangerServer.Modules
{
    public class JsonParser
    {
        private JsonTextParser jsonTextParser;
        private JsonObjectCollection jsonObjectCollection;

        public JsonParser(string data)
        {
            this.jsonTextParser = new JsonTextParser();
            this.jsonObjectCollection = (JsonObjectCollection)jsonTextParser.Parse(data);
        }

        public string GetStringValue(string name)
        {
            string value = string.Empty;

            try
            {
                value = Convert.ToString(jsonObjectCollection[name].GetValue());
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return value;
        }

        public string[] GetStringArrayValue(string name)
        {
            string[] arrayValue = null;

            try
            {
                JsonArrayCollection jsonArrayCollection = (JsonArrayCollection)jsonObjectCollection[name];
                int count = jsonArrayCollection.Count;

                arrayValue = new string[count];

                for (int i = 0; i < count; i++)
                {
                    arrayValue[i] = ((JsonStringValue)jsonArrayCollection[i]).Value;
                }     
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return arrayValue;
        }
    }
}
