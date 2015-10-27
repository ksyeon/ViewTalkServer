using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using WakeUpMessangerServer.Configs;

namespace WakeUpMessangerServer.Modules
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

        public int GetUserNumber(string id)
        {
            string query = $"SELECT no FROM user WHERE id = '{id}'";
            DataSet result = dbConnector.SelectQuery(query);

            int userNumber = Convert.ToInt32(result.Tables[0].Rows[0]["no"]);

            return userNumber;
        }
    }
}
