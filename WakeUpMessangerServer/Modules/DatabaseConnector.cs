using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using MySql.Data.MySqlClient;

namespace WakeUpMessangerServer.Modules
{
    public class DatabaseConnector
    {
        private MySqlConnection dbConnection;

        public DatabaseConnector(string server, string database, string userID, string password)
        {
            try
            {
                string connInfo = $"Server={server};Database={database};Uid={userID};Pwd={password};Charset=utf8";
                this.dbConnection = new MySqlConnection(connInfo);

                Console.WriteLine("Database Connect Success");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void SendQuery(string query)
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

        public DataSet SelectQuery(string query)
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

        public int CountRow(string query)
        {
            DataSet dataSet = SelectQuery(query);

            return dataSet.Tables[0].Rows.Count;
        }

        public bool IsExistRow(string query)
        {
            bool isExist = false;
            int count = CountRow(query);

            if (count > 0)
            {
                isExist = true;
            }

            return isExist;
        }
    }
}
