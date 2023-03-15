using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Utility.Configuration;

namespace AppZoneMiddleware.Shared.Utility.ConnectionProcessor
{
    public class DataAccessComponent
    {
        private IDbConnection dbConnection;
        private IDbCommand dbcommand;

        public DataAccessComponent(string processorKey)
        {
            dbConnection = new System.Data.OleDb.OleDbConnection(ConfigurationManager.ProcessorConnectionString[processorKey]); //new System.Data.OleDb.OleDbConnection(Configuration.ConfigurationManager.CBAProcessorConnections);
            dbcommand = new System.Data.OleDb.OleDbCommand();
        }
        public IDbConnection Connection { get { return dbConnection; } }
        public IDbCommand Command { get { return dbcommand; } }
    }
}
