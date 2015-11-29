using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Json;

using ViewTalkServer.Models;

namespace ViewTalkServer.Modules
{
    public class JsonHelper
    {
        private DatabaseHelper database;

        public JsonHelper()
        {
            this.database = new DatabaseHelper();
        }

        public Dictionary<string, string> GetLoginInfo(string data)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            JsonParser jsonParser = new JsonParser(data);

            result.Add(JsonName.ID, jsonParser.GetStringValue(JsonName.ID));
            result.Add(JsonName.Password, jsonParser.GetStringValue(JsonName.Password));

            return result;
        }

        public string SetChattingUser(List<ClientData> client, int chatNumber) // 수정 필요.
        {
            JsonObjectCollection result = new JsonObjectCollection();

            foreach(ClientData chattingUser in client)
            {
                if(chattingUser.Group == chatNumber)
                {
                    string strUserName = Convert.ToString(chattingUser.Number);
                    string nickname = database.GetNickName(chattingUser.Number);

                    result.Add(new JsonStringValue(strUserName, nickname));
                }
            }

            return result.ToString();
        }
    }

    public class JsonName
    {
        public const string ID = "ID";
        public const string Password = "Password";
    }
}
