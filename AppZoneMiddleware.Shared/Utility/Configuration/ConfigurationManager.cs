using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility.Configuration
{
    public class ConfigurationManager
    {
        public static NameValueCollection ProcessorConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("BlendMiddleware.ProcessorConnections") as NameValueCollection;
            }
        }

        private static NameValueCollection CBAEntities
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("ProvidusMiddleware.CBAEntities") as NameValueCollection;
            }
        }

        public static string GetAccountStatement
        {
            get
            {
                return CBAEntities["GetAccountStatement"];
            }
        }
    }
}
