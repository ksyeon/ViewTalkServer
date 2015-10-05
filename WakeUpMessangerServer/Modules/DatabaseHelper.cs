using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WakeUpMessangerServer.Modules
{
    class DatabaseHelper
    {
        DatabaseConnector dbConnector;

        public DatabaseHelper()
        {
            this.dbConnector = new DatabaseConnector("server", "database", "id", "password");
        }
    }
}
