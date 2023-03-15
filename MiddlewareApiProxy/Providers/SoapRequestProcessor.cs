using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Web;
using System.Web.Services.Description;

namespace MiddlewareApiProxy.Providers
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

        static CodeCompileUnit GenerateCSCodeForService(string metadataAddress)
        {
            #region Get Mex from endpoint URL
            //MetadataExchangeClient mexClient = new MetadataExchangeClient(metadataAddress, MetadataExchangeClientMode.HttpGet);
            //mexClient.ResolveMetadataReferences = true;
            //MetadataSet metaDocs = mexClient.GetMetadata(); 
            #endregion

            System.Web.Services.Description.ServiceDescription description = System.Web.Services.Description.ServiceDescription.Read(metadataAddress);
            var sec = MetadataSection.CreateFromServiceDescription(description);
            var metaDocs = new MetadataSet(new List<MetadataSection> { sec });


            WsdlImporter importer = new WsdlImporter(metaDocs);
            ServiceContractGenerator generator = new ServiceContractGenerator()
            {
                Options = ServiceContractGenerationOptions.ClientClass | ServiceContractGenerationOptions.TypedMessages
            };

            #region Unnecesary code for contract importer
            //  // Add our custom DCAnnotationSurrogate 
            //  // to write XSD annotations into the comments.
            //  object dataContractImporter;
            //  XsdDataContractImporter xsdDCImporter;
            //  if (!importer.State.TryGetValue(typeof(XsdDataContractImporter), out dataContractImporter))
            //  {
            //      Console.WriteLine("Couldn't find the XsdDataContractImporter! Adding custom importer.");
            //      xsdDCImporter = new XsdDataContractImporter();
            //      xsdDCImporter.Options = new ImportOptions();
            //      importer.State.Add(typeof(XsdDataContractImporter), xsdDCImporter);
            //  }
            //  else
            //  {
            //      xsdDCImporter = (XsdDataContractImporter)dataContractImporter;
            //      if (xsdDCImporter.Options == null)
            //      {
            //          Console.WriteLine("There were no ImportOptions on the importer.");
            //          xsdDCImporter.Options = new ImportOptions()
            //          {

            //          };
            //      }
            //  }
            // // xsdDCImporter.Options.DataContractSurrogate = new DCAnnotationSurrogate();

            //  // Uncomment the following code if you are going to do your work programmatically rather than add 
            //  // the WsdlDocumentationImporters through a configuration file. 

            //  // The following code inserts a custom WsdlImporter without removing the other 
            //  // importers already in the collection.
            //  System.Collections.Generic.IEnumerable<IWsdlImportExtension> exts = importer.WsdlImportExtensions;
            //  System.Collections.Generic.List<IWsdlImportExtension> newExts 
            //    = new System.Collections.Generic.List<IWsdlImportExtension>();
            //  foreach (IWsdlImportExtension ext in exts)
            //  {
            //    Console.WriteLine("Default WSDL import extensions: {0}", ext.GetType().Name);
            //    newExts.Add(ext);
            //  }
            ////  newExts.Add(new WsdlDocumentationImporter());
            //  System.Collections.Generic.IEnumerable<IPolicyImportExtension> polExts = importer.PolicyImportExtensions;
            //  importer = new WsdlImporter(metaDocs, polExts, newExts);

            #endregion

            System.Collections.ObjectModel.Collection<ContractDescription> contracts
              = importer.ImportAllContracts();
            importer.ImportAllEndpoints();
            foreach (ContractDescription contract in contracts)
            {
                generator.GenerateServiceContractType(contract);
            }
            if (generator.Errors.Count != 0)
                throw new Exception("There were errors during code compilation.");

            //Write the code dom
            //CodeGeneratorOptions options = new CodeGeneratorOptions() { BracingStyle = "C" };
            //CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            //IndentedTextWriter textWriter = new IndentedTextWriter(new StreamWriter("CodeDom.cs"));
            //codeDomProvider.GenerateCodeFromCompileUnit(
            //  generator.TargetCompileUnit, textWriter, options
            //);
            return generator.TargetCompileUnit;
        }
    }
}