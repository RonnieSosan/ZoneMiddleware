using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Blend.DefaultImplementation.Utility
{
    public class SoapRequestProcessor
    {
        public object ProcessRequest(string OperationName, string MethodName, string requestMessage, string URLAddress)
        {
            Logger.LogInfo("MessageBroker.SoapRequestProcessor.ProcessRequest: request", string.Format("request: {0}, URL: {1}", requestMessage, URLAddress));
            try
            {
                #region Old implementation
                //var unit = GenerateCSCodeForService(wsdlAddress);

                //// Generate the proxy code
                //CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                //// Compile the assembly proxy with the appropriate references
                //var assemblyReferences = new CompilerParameters(new[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" });
                //assemblyReferences.GenerateExecutable = false;

                ////CompilerParameters parms = new CompilerParameters(assemblyReferences);
                //assemblyReferences.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);

                //CompilerResults results = provider1.CompileAssemblyFromDom(assemblyReferences, unit);

                //// Check For Errors
                //if (results.Errors.Count > 0)
                //{
                //    foreach (CompilerError oops in results.Errors)
                //    {
                //        Logger.LogError(new Exception("========Compiler error============"));
                //        Logger.LogError(new Exception(oops.ErrorText));
                //    }
                //    throw new System.Exception("Compile Error Occured calling webservice. Check log file.");
                //} 
                #endregion

                // Finally, Invoke the web service method
                Logger.LogInfo("AssemblyInfo", ServiceDescriptionAssembler._assemblies);
                if (!ServiceDescriptionAssembler._assemblies.ContainsKey(URLAddress))
                {
                    throw new Exception(string.Format("Cannot find Assembly for Service: {0}", URLAddress));
                }
                Assembly assembly = ServiceDescriptionAssembler._assemblies[URLAddress];

                object wsvcClass = assembly.CreateInstance(OperationName, false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress(URLAddress)
                }, null, null);

                if (wsvcClass == null)
                {
                    throw new Exception(string.Format("Cannot find Operation Specified: {0}", OperationName));
                }

                MethodInfo mi = wsvcClass.GetType().GetMethod(MethodName);

                if (mi == null)
                {
                    throw new Exception(string.Format("Cannot find web method Specified: {0}", MethodName));
                }
                var request = JsonConvert.DeserializeObject(requestMessage, mi.GetParameters()[0].ParameterType);

                object rtn = mi.Invoke(wsvcClass, new object[] { request });
                Logger.LogInfo("MessageBroker.SoapRequestProcessor.ProcessRequest: RawResponse", rtn);
                //Get actual return value from response
                Type myType = rtn.GetType();
                FieldInfo returnProperty = myType.GetFields()[0];
                object returnValue = returnProperty.GetValue(rtn);

                Logger.LogInfo("MessageBroker.SoapRequestProcessor.ProcessRequest: ResponseDeserialized", returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                throw ex.InnerException != null ? ex.InnerException : ex;
            }
        }
    }
}
