using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using ViewTalkServer.Configs;

namespace ViewTalkServer.Modules
{
    public class DatabaseHelper
    {
        DatabaseConnector dbConnector;

        public DatabaseHelper()
        {
            string server = DatabaseConfig.Server;
            string database = DatabaseConfig.Database;
            string userId = DatabaseConfig.UserId;
            string password = DatabaseConfig.Password;

            this.dbConnector = new DatabaseConnector(server, database, userId, password);
        }

        public bool IsExistUser(string id, string password)
        {
            string query = $"SELECT * FROM user WHERE id='{id}' AND password = password('{password}')";
            bool result = dbConnector.IsExistRow(query);

            return result;
        }

        public bool IsExistId(string id)
        {
            string query = $"SELECT * FROM user WHERE id='{id}'";
            bool result = dbConnector.IsExistRow(query);

            return result;
        }

        public bool IsExistNickname(string nickname)
        {
            string query = $"SELECT * FROM user WHERE nickname='{nickname}'";
            bool result = dbConnector.IsExistRow(query);

            return result;
        }

        public int GetNumberOfId(string id)
        {
            string query = $"SELECT no FROM user WHERE id = '{id}'";
            DataSet result = dbConnector.SelectQuery(query);

            int userNumber = Convert.ToInt32(result.Tables[0].Rows[0]["no"]);

            return userNumber;
        }

        public int GetNumberOfNickname(string nickname)
        {
            string query = $"SELECT no FROM user WHERE nickname = '{nickname}'";
            DataSet result = dbConnector.SelectQuery(query);

            int userNumber = Convert.ToInt32(result.Tables[0].Rows[0]["no"]);

            return userNumber;
        }

        public string GetIdOfNumber(int number)
        {
            string query = $"SELECT id FROM user WHERE no = '{number}'";
            DataSet result = dbConnector.SelectQuery(query);

            string id = Convert.ToString(result.Tables[0].Rows[0]["id"]);

            return id;
        }

        public string GetNickNameOfNumber(int number)
        {
            string query = $"SELECT nickname FROM user WHERE no = '{number}'";
            DataSet result = dbConnector.SelectQuery(query);

            string userNickName= Convert.ToString(result.Tables[0].Rows[0]["nickname"]);

            return userNickName;
        }
    }
}
