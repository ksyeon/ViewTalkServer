﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WakeUpMessangerServer.Configs;

namespace WakeUpMessangerServer.Modules
{
    class DatabaseHelper
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
    }
}
