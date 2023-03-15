using System;
using NLog;
using NLog.Targets;
using NLog.Config;
using Elasticsearch;
using Nest;
using Elasticsearch.Net;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AppZoneMiddleware.Shared.Utility
{
    public class ElasticSearchLogger
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static void Log(DateTime requestTime, string category, string resourceName, string routeUrl,
            long timeTaken)
        {
            //var message = $"{requestTime} {category} {resourceName} {routeUrl} {timeTaken}";
            //logger.Info(message);
        }
        public static void Log(Exception ex, string message = null)
        {
            logger.Error(ex, message);
        }
        public static void Log(string message)
        {
            logger.Info(message);
        }

        public async Task LogMessage(string ActionName, Dictionary<string, string> message, string requestID)
        {
            string indexName = ConfigurationManager.AppSettings["ElasticIndex"];
            string type = ConfigurationManager.AppSettings["ElasticType"];
            //LogManager.Configuration = new XmlLoggingConfiguration(string.Format(@"{0}\DejaVu.Host.exe.config", AppDomain.CurrentDomain.BaseDirectory));
            //FileTarget target = LogManager.Configuration.FindTargetByName("file") as FileTarget;
            //string curDate = DateTime.Now.ToString("ddd(dd)-MM-yyyy");
            try
            {
                //String logfile = string.Format("C:\\CommandLogs\\{0}\\{1}.txt", ActionName.Replace(" ", "_"), curDate);
                //target.FileName = logfile;
                //logger.Info(message);
                var logData = new ActionAttributes
                {
                    ActionName = ActionName,
                    Message = message,
                    TimeLogged = DateTime.Now,
                    RequestID = requestID
                };

               // Dictionary<string, object> LogData = new Dictionary<string, object> { { "ActionName", "ActionName" }, { "TimeLogged", DateTime.Now }, { "RequestID", requestID } };


                if (!EsClient().IndexExists(indexName).Exists)
                {
                    await EsClient().CreateIndexAsync(indexName, c => c
                    .Mappings(m => m.Map<ActionAttributes>(mp => mp.AutoMap())));
                }

                var response = await EsClient().IndexAsync(logData, i => i
                .Index(indexName)
                .Type(type));
            }
            catch (Exception ex)
            {
                //String logfile = string.Format("C:\\CommandLogs\\{0}\\{1}.txt", "_ErrorLog", curDate);
                //target.FileName = logfile;
                logger.Error(string.Format("Error Details: {0} {1} {2}", ex.InnerException, ex.Message, ex.StackTrace));
            }
        }

        public static ElasticClient EsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;

            //Multiple node for fail over (cluster addresses)
            var nodes = new Uri[]
            {
                new Uri(ConfigurationManager.AppSettings.Get("ElasticSearchUrl"))
            };

            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool);
            elasticClient = new ElasticClient(connectionSettings);
            return elasticClient;
        }
    }

    public class ActionAttributes
    {
        public string ActionName { get; set; }
        public Dictionary<string, string> Message { get; set; }
        public DateTime TimeLogged { get; set; }
        public string RequestID { get; set; }
    }
}
