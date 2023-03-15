using AppZoneMiddleware.Shared.Entities;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared.Utility
{
    public class Logger
    {
        static string fileNameFormat = string.Format("{0}//internetBankingErrors_{1:dd-MMM-yyyy}.txt", System.Configuration.ConfigurationManager.AppSettings["Logfile"], DateTime.Now);
        static bool enableRemoteLogger = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["EnableRemoteLogger"]);
        static string LoggerUrl = System.Configuration.ConfigurationManager.AppSettings["LoggerUrl"];

        public static void Info(string v, LifestyleDebitRequest request)
        {
            throw new NotImplementedException();
        }

        static EventLog _eventLog = new EventLog();
        private const string EVENT_LOG_SOURCE = "Blend Middleware Notification Event";
        private const string EVENT_LOG_NAME = "AppZoneBlendMiddlewareLog";

        static Logger()
        {
            /**
             * NOTE: You will need to create the Log Name and Log Source manually, if this is being run from a Web Application. 
             * This is because the code below (for creating the event log and source) requires administrator privileges.
             * To create the Log Name and Log Source manually, open Registry editor and then perform the following actions: 
             * 1. Locate HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog
             * 2. Add a new Key named "AppZoneBlendMiddlewareLog" under the "eventlog" key.
             * 3. Add a new Key named "Blend Middleware Notification Event" under the "AppZoneBlendMiddlewareLog" key.
             * 4. In the "Blend Middleware Notification Event" key, add an Expandable String Value named "EventMessageFile".
             * 5. Edit the value of the entry in (4) above, and set it's "Value data" to: C:\Windows\Microsoft.NET\Framework\v4.0.30319\EventLogMessages.dll
             * 6. Restart the OS. 
             * 7. Launch this Web App and valdiate that log entries are created. 
             **/

            try
            { 
                if (!EventLog.Exists(EVENT_LOG_NAME))
                    EventLog.CreateEventSource(EVENT_LOG_SOURCE, EVENT_LOG_NAME);
                EventLog.WriteEntry(EVENT_LOG_SOURCE, $"{typeof(Logger).FullName} has been initialized.");
            }
            catch (Exception) { }
            finally
            {
                _eventLog.Source = EVENT_LOG_SOURCE;
                _eventLog.Log = EVENT_LOG_NAME;
            }
        }

        public static void LogInfo(string MethodName, object message)
        {
            message = message.GetType() != typeof(string) ? Newtonsoft.Json.JsonConvert.SerializeObject(message) : message;
            new TaskFactory().StartNew(() => LogInfoHelper(MethodName, message, 0));
        }
        public static void LogError(Exception ex)
        {
            new TaskFactory().StartNew(() => LogErrorHelper(ex, 2));
        }


        private static void LogInfoHelper(string MethodName, object message, int recursionCount)
        {
            if (recursionCount > 50)
                return;

            string LogDate = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss:fff");
            StringBuilder sb = new StringBuilder();
            try
            {
                string filename = string.Format(fileNameFormat, System.Configuration.ConfigurationManager.AppSettings["Logfile"], DateTime.Now);
                sb.AppendLine(LogDate);
                sb.AppendLine("caller: " + MethodName + "\n: " + message);

                using (System.IO.StreamWriter str = new System.IO.StreamWriter((recursionCount > 0 ? GetNewFileName(filename, recursionCount) : filename), true))
                {
                    str.WriteLine(sb.ToString());
                }
            }
            catch
            {
                recursionCount++;
                LogInfoHelper(MethodName, message, recursionCount);
            }

            string institution = System.Configuration.ConfigurationManager.AppSettings["ElasticIndex"];
            JObject serverMsg = new JObject() { { "MethodName", MethodName }, { "Message", message.ToString() }, { "DateTime", LogDate }, { "Institution", institution} };

            ReportErrorToServer(serverMsg);
        }

        private static void LogErrorHelper(Exception ex, int recursionCount)
        {
            if (recursionCount > 50)
                return;

            StringBuilder sb = new StringBuilder();
            string dateTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss:fff");
            try
            {
                string filename = string.Format(fileNameFormat, System.Configuration.ConfigurationManager.AppSettings["Logfile"], DateTime.Now);
                string message = GetExceptionMessages(ex);
                string stackTrace = ex.StackTrace;
                sb.AppendLine("Date and Time: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss:fff"));
                sb.AppendLine("ErrorMessage: \n " + message);
                sb.AppendLine("StackTrace: \n " + stackTrace);

                using (System.IO.StreamWriter str = new System.IO.StreamWriter((recursionCount > 0 ? GetNewFileName(filename, recursionCount) : filename), true))
                {
                    str.WriteLine(sb.ToString());
                }
            }
            catch
            {
                recursionCount++;
                //cannot log file to in catch as it can cause a stackOverFlow exception
                //LogErrorHelper(ex, recursionCount);
            }

            ReportErrorToServer(new JObject { { "MethodName", "ErrorMessage:" }, { "Message", GetExceptionMessages(ex) }, { "DateTime", dateTime } });
        }


        private static string GetNewFileName(string fullFileName, int recursionCount)
        {
            int indexOfPeriod = fullFileName.LastIndexOf(".");
            string newFileName = string.Format("{0}_{1}{2}", fullFileName.Substring(0, indexOfPeriod), recursionCount, fullFileName.Substring(indexOfPeriod));
            return newFileName;
        }

        static string GetExceptionMessages(Exception ex)
        {
            string ret = string.Empty;
            if (ex != null)
            {
                ret = ex.Message;
                if (ex.InnerException != null)
                    ret = ret + "\n" + GetExceptionMessages(ex.InnerException);
            }
            return ret;
        }

        public static async void ReportErrorToServer(JObject msg)
        {
            try
            {
                if (enableRemoteLogger == false) return;

                HttpContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(msg));

                using (HttpClient http = new HttpClient())
                {
                    //  HttpClient http = new HttpClient();
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, LoggerUrl);
                    req.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    req.Content = content;
                    req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    var resp = await http.SendAsync(req);
                    var responseContent = await resp.Content.ReadAsStringAsync();
                }
            }
            catch (Exception e)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Exception While Report-Logging is: ");
                    sb.AppendLine(GetExceptionMessages(e));
                    _eventLog.WriteEntry("An ERROR has occured while trying to log to file.\n\n\n" + sb.ToString() + "\n\nThe error occured at " + e.StackTrace + "\n\n" + e.InnerException, EventLogEntryType.FailureAudit);
                }
                catch (Exception) { }
            }
        }
    }
}
