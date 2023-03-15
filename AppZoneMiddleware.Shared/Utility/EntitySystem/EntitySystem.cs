using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility.EntitySystem
{
    public class EntitySystem<T> : DAOSystem
    {
        bool isDemo = false;
        public EntitySystem(string ProcessorKey)
            : base(ProcessorKey)
        {
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["Isdemo"], out isDemo);
        }


        public IList<T> GetEntityList(string queryString)
        {
            OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString); //new System.Data.OleDb.OleDbCommand(Configuration.ConfigurationManager.PNDDetails.Replace(":ACCOUNT_NUMBER", "'" + acc_key + "'"));
            return RetrieveList<T>(command);
        }

        public T GetEntity(string queryString)
        {
            OleDbCommand command = new System.Data.OleDb.OleDbCommand(queryString); //new System.Data.OleDb.OleDbCommand(Configuration.ConfigurationManager.PNDDetails.Replace(":ACCOUNT_NUMBER", "'" + acc_key + "'"));
            return RetrieveSingle<T>(command);
        }

        public string RetrieveBySpecificProperty(string query, string columnName)
        {
            OleDbCommand theCommand = new System.Data.OleDb.OleDbCommand(query);
            return RetrieveSpecificProperty(theCommand, columnName);
        }
    }
}
