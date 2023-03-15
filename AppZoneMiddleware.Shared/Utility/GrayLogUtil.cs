using Serilog;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    public class GrayLogUtil
    {
        private static object syncObject = new Object();
        private static volatile GrayLogUtil _instance = null;
        private static readonly string CLASS_NAME = typeof(GrayLogUtil).FullName;
        private static volatile ILogger _loggerInstance = null;
        private static string GrayLogHostnameOrAddress
        {
            get { return Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["GrayLogHostnameOrAddress"]); }
        }

        private static int GrayLogPort
        {
            get { return Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["GrayLogPort"]); }
        }

        private GrayLogUtil()
        {
            System.Diagnostics.Trace.TraceInformation("In {0} constructor!", GrayLogUtil.CLASS_NAME);
            try
            {
                Serilog.Debugging.SelfLog.Enable(msg => System.Diagnostics.Trace.TraceError(msg));
                var loggerConfig = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Graylog(new GraylogSinkOptions()
                    {
                        MinimumLogEventLevel = Serilog.Events.LogEventLevel.Verbose,
                        HostnameOrAdress = GrayLogHostnameOrAddress,
                        Facility = GrayLogUtil.CLASS_NAME,
                        Port = GrayLogPort,
                        TransportType = TransportType.Http
                    });

                _loggerInstance = loggerConfig.CreateLogger();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Exception In {0}! StackTrace: {1}", GrayLogUtil.CLASS_NAME, ex.StackTrace);
            }
            System.Diagnostics.Trace.TraceInformation("Exiting {0} constructor!", GrayLogUtil.CLASS_NAME);
        }

        public static GrayLogUtil ClassInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        if (_instance == null)
                            _instance = new GrayLogUtil();
                    }
                }

                return _instance;
            }
        }

        public static ILogger LoggerInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncObject)
                    {
                        if (_instance == null || _loggerInstance == null)
                            _instance = new GrayLogUtil();
                    }
                }

                return _loggerInstance;
            }
        }
    }
}
