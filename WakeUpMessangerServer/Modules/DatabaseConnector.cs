using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using MySql.Data.MySqlClient;

namespace WakeUpMessangerServer.Modules
{
    class DatabaseConnector
    {
        private MySqlConnection dbConnection;

        public DatabaseConnector(string server, string database, string userID, string password)
        {
            try
            {
                string connInfo = $"Server={server};Database={database};Uid={userID};Pwd={password};Charset=utf8";
                this.dbConnection = new MySqlConnection(connInfo);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void sendQuery(string query)
        {
            try
            {
                dbConnection.Open();

                MySqlCommand command = new MySqlCommand(query, dbConnection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        public DataSet selectQuery(string query)
        {
            DataSet dataSet = new DataSet();

            try
            {
                dbConnection.Open();

                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(query, dbConnection);
                dataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dbConnection.Close();
            }

            return dataSet;
        }
    }
}
