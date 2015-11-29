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

        public string SetChattingUser(List<ClientData> clientList, int chatNumber)
        {
            JsonObjectCollection result = new JsonObjectCollection();

            JsonArrayCollection userNumberArray = new JsonArrayCollection(JsonName.UserNumber);
            JsonArrayCollection nicknameArray = new JsonArrayCollection(JsonName.Nickname);

            foreach (ClientData chattingUser in clientList)
            {
                if(chattingUser.Group == chatNumber)
                {
                    string userNumber = Convert.ToString(chattingUser.Number);
                    string nickname = database.GetNickNameOfNumber(chattingUser.Number);

                    userNumberArray.Add(new JsonStringValue(null, userNumber));
                    nicknameArray.Add(new JsonStringValue(null, nickname));
                }
            }

            result.Add(userNumberArray);
            result.Add(nicknameArray);

            return result.ToString();
        }
    }

    public class JsonName
    {
        public const string ID = "ID";
        public const string Password = "Password";
        public const string UserNumber = "UserNumber";
        public const string Nickname = "Nickname";
    }
}
