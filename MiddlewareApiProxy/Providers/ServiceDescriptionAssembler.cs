using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using MiddlewareApiProxy.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Web;

namespace MiddlewareApiProxy.Providers
{
    public class ServiceDescriptionAssembler
    {
        public static Dictionary<string, Assembly> _assemblies;
        public string EntityAPIEndpoint = System.Configuration.ConfigurationManager.AppSettings.Get("EntityAPIEndpoint");

        public static void Initialize()
        {
            _assemblies = new Dictionary<string, Assembly>();
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            List<ApiSecuritySpec> securitySpecs = repository.Get().Where(x => x.isSoap).ToList();
            foreach (var item in securitySpecs)
            {
                try
                {
                    System.Web.Services.Description.ServiceDescription description = null;
                    using (TextReader sr = new StringReader(item.ServiceDescription))
                    {
                        description = System.Web.Services.Description.ServiceDescription.Read(sr);
                    }
                    var sec = MetadataSection.CreateFromServiceDescription(description);
                    var metaDocs = new MetadataSet(new List<MetadataSection> { sec });

                    WsdlImporter importer = new WsdlImporter(metaDocs);
                    ServiceContractGenerator generator = new ServiceContractGenerator()
                    {
                        Options = ServiceContractGenerationOptions.ClientClass | ServiceContractGenerationOptions.TypedMessages
                    };
                    Collection<ContractDescription> contracts = importer.ImportAllContracts();
                    importer.ImportAllEndpoints();

                    foreach (ContractDescription contract in contracts)
                    {
                        generator.GenerateServiceContractType(contract);
                    }
                    if (generator.Errors.Count != 0)
                    {
                        Logger.LogError(new Exception("There were errors during code compilation."));
                        continue;
                    }

                    ////Generate the proxy code
                    CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                    // Compile the assembly proxy with the appropriate references
                    var assemblyReferences = new CompilerParameters(new[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll", "System.Runtime.Serialization.dll" });
                    assemblyReferences.GenerateExecutable = false;

                    //CompilerParameters parms = new CompilerParameters(assemblyReferences);
                    assemblyReferences.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);

                    CompilerResults results = provider1.CompileAssemblyFromDom(assemblyReferences, generator.TargetCompileUnit);

                    //Check For Errors
                    if (results.Errors.Count > 0)
                    {
                        foreach (CompilerError oops in results.Errors)
                        {
                            Logger.LogError(new Exception("========Compiler error============"));
                            Logger.LogError(new Exception(oops.ErrorText));
                        }
                        throw new Exception("Compile Error Occured calling webservice. Check log file.");
                    }
                    Assembly serviceAssembly = results.CompiledAssembly;

                    _assemblies.Add(item.URL, serviceAssembly);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.InnerException == null ? ex : ex.InnerException);
                    continue;
                }

            }
        }

        public void GetServiceContracts(GetServiceDescriptionRequest request)
        {

        }

        public GenerateOperationsResponse GenerateServiceOperations(GenerateServiceDescriptionRequest request)
        {
            List<ServiceOperations> operations = new List<ServiceOperations>();
            GenerateOperationsResponse response = new GenerateOperationsResponse();
            GetServiceDescriptionResponse service = GenerateServiceDescription(request.Format, request.URL, request.ServiceDocs);

            if (service.ClientName == null)
            {
                response = new GenerateOperationsResponse { ResponseCode = "06", ResponseDescription = "Unable to generate service description, please confirm service details" };
                return response;
            }
            GenerateOrleansOperationReq orleansReq = new GenerateOrleansOperationReq();
            orleansReq.Operations = new List<ServiceOperations>();
            foreach (var item in request.Operations)
            {
                var operation = service.Operations.Where(x => x.OperationName == item).FirstOrDefault();
                if (operation != null)
                    orleansReq.Operations.Add(operation);
                else
                    Logger.LogInfo("GenerateServiceOperations", string.Format("Unable to find method {0}", item));
            }

            if (orleansReq.Operations.Count == 0)
            {
                response = new GenerateOperationsResponse { ResponseCode = "06", ResponseDescription = "no operations fround from your list of operations specified, confirm your wsdl and request message" };
                return response;
            }
            orleansReq.ClientName = service.ClientName;
            orleansReq.ConnectorName = request.ConnectorName;
            orleansReq.ForceGeneration = true;
            orleansReq.URL = request.URL;
            orleansReq.ContractName = service.ContractName;
            orleansReq.InstitutionCode = request.InstitutionCode;

            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage() { RequestUri = new Uri(EntityAPIEndpoint), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(orleansReq), Encoding.UTF8, "application/json") };
                HttpResponseMessage httpResponse = client.SendAsync(httpRequest).Result;
                string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;

                if (!httpResponse.IsSuccessStatusCode)
                {
                    // This will result in an Exception if the HTTP Status Code is not successful. 
                    try
                    {

                        JObject jsonObject = JsonConvert.DeserializeObject<JObject>(rawResponse);
                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "06", ResponseDescription = $"Filed Server Response ({httpResponse.StatusCode}): {Convert.ToString(jsonObject["Message"])}" });
                    }
                    catch (Exception)
                    {
                        rawResponse = JsonConvert.SerializeObject(new { ResponseCode = "06", ResponseDescription = rawResponse });
                    }

                }
                else
                {
                    JObject success = JsonConvert.DeserializeObject<JObject>(rawResponse);
                    response = new GenerateOperationsResponse { ResponseCode = "00", ResponseDescription = "SUCCESSFUL", GeneratedOperations = success };
                }
            }
            return response;

        }

        public GetServiceDescriptionResponse GetForSoap(GetServiceDescriptionRequest request)
        {
            string ServiceName = string.Empty;
            GetServiceDescriptionResponse service = GenerateServiceDescription(request.Format, request.URL, request.ServiceDocs);
            return service;
        }

        //public GetServiceDescriptionResponse GenerateServiceOperations(GenerateOperationsRequests request)
        //{

        //}

        private GetServiceDescriptionResponse GenerateServiceDescription(ServicDescriptionFormat servicDescriptionFormat, string URL, string ServiceDocs)
        {
            GetServiceDescriptionResponse service = null;
            try
            {
                service = new GetServiceDescriptionResponse();
                System.Web.Services.Description.ServiceDescription description = null;
                MetadataSet metaDocs = null;
                //switch between methods of service description generation 
                switch (servicDescriptionFormat)
                {
                    case ServicDescriptionFormat.SoapXml:
                        using (TextReader sr = new StringReader(ServiceDocs))
                        {
                            description = System.Web.Services.Description.ServiceDescription.Read(sr);
                            var sec = MetadataSection.CreateFromServiceDescription(description);
                            metaDocs = new MetadataSet(new List<MetadataSection> { sec });

                        }
                        break;
                    case ServicDescriptionFormat.SoapUrl:
                        string rawServiceDescription = string.Empty;
                        string serviceUri = "?wsdl";
                        if (URL.Contains(".svc")) serviceUri = "?singlewsdl";
                        HttpRequestMessage requestHttp = new HttpRequestMessage(HttpMethod.Get, new Uri(URL + serviceUri));
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage serverResponse = new HttpResponseMessage();
                            serverResponse = client.SendAsync(requestHttp).Result;
                            rawServiceDescription = serverResponse.Content.ReadAsStringAsync().Result;
                        }
                        using (TextReader sr = new StringReader(rawServiceDescription))
                        {
                            description = System.Web.Services.Description.ServiceDescription.Read(sr);
                            var metadataSection = MetadataSection.CreateFromServiceDescription(description);
                            metaDocs = new MetadataSet(new List<MetadataSection> { metadataSection });
                        }
                        break;
                    default:
                        break;
                }

                WsdlImporter importer = new WsdlImporter(metaDocs);
                ServiceContractGenerator generator = new ServiceContractGenerator()
                {
                    Options = ServiceContractGenerationOptions.ClientClass | ServiceContractGenerationOptions.TypedMessages
                };
                ContractDescription contract = importer.ImportAllContracts().FirstOrDefault();

                service.ClientName = contract.Name + "Client";
                service.ContractName = contract.Name;
                List<string> listOfOperations = new List<string>();
                OperationDescriptionCollection operations = contract.Operations;
                foreach (var operation in operations)
                {
                    listOfOperations.Add(operation.Name);
                }


                generator.GenerateServiceContractType(contract);
                if (generator.Errors.Count != 0)
                {
                    Logger.LogError(new Exception("There were errors during code compilation."));
                }

                ////Generate the proxy code
                CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                // Compile the assembly proxy with the appropriate references
                var assemblyReferences = new CompilerParameters(new[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll", "System.Runtime.Serialization.dll" });
                assemblyReferences.GenerateExecutable = false;

                //CompilerParameters parms = new CompilerParameters(assemblyReferences);
                assemblyReferences.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);

                CompilerResults results = provider1.CompileAssemblyFromDom(assemblyReferences, generator.TargetCompileUnit);

                //Check For Errors
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError oops in results.Errors)
                    {
                        Logger.LogError(new Exception("========Compiler error============"));
                        Logger.LogError(new Exception(oops.ErrorText));
                    }
                    throw new Exception("Compile Error Occured calling webservice. Check log file.");
                }
                Assembly serviceAssembly = results.CompiledAssembly;
                object wsvcClass = null;
                try
                {
                    var hfoid = serviceAssembly.DefinedTypes.Where(x => x.Name == "Service1Soap");
                    wsvcClass = results.CompiledAssembly.CreateInstance(service.ClientName, false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress("http://10.0.33.62/EntrustBridge/API.svc") }, null, null);

                    if (wsvcClass == null)
                    {
                        wsvcClass = results.CompiledAssembly.CreateInstance(service.ClientName.Replace("Client", ""), false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress("http://10.0.33.62/EntrustBridge/API.svc")}, null, null);
                    }

                    if (wsvcClass == null)
                    {
                        wsvcClass = results.CompiledAssembly.CreateInstance(service.ClientName.Replace("Client", "Soap"), false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress("http://10.0.33.62/EntrustBridge/API.svc") }, null, null);
                    }
                }
                catch (Exception ex)
                {
                    wsvcClass = results.CompiledAssembly.CreateInstance(service.ClientName.Replace("Client", "SoapClient"), false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress("http://10.0.33.62/EntrustBridge/API.svc") }, null, null);
                }




                //Read through the list of contracts
                List<ServiceOperations> arrayOfOperations = new List<ServiceOperations>();
                //loop through the list of operations
                foreach (var operation in listOfOperations)
                {
                    MethodInfo mi = wsvcClass.GetType().GetMethod(operation);
                    ServiceOperations _operation = new ServiceOperations();
                    _operation.OperationName = mi.Name;
                    var responseType = mi.ReturnParameter.ParameterType;
                    var requestType = mi.GetParameters().FirstOrDefault().ParameterType;
                    ObjectGenerator objectGenerator = new ObjectGenerator();
                    var sampleRequest = objectGenerator.GenerateObject(requestType);
                    var sampleResponse = objectGenerator.GenerateObject(responseType);

                    //Generate properties 
                    ObjectPropertyGenerator objectifier = new ObjectPropertyGenerator();
                    var requestProperties = objectifier.GenerateObject(requestType);
                    var responseProperties = objectifier.GenerateObject(responseType);
                    _operation.RequestMessageProperties = requestProperties as Dictionary<string, object>;
                    _operation.ResponseMessageProperties = responseProperties as Dictionary<string, object>;
                    var samples = new Dictionary<MediaTypeHeaderValue, object>();
                    Collection<MediaTypeFormatter> formatters;
                    formatters = new Collection<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
                    foreach (var formatter in formatters)
                    {
                        foreach (MediaTypeHeaderValue mediaType in formatter.SupportedMediaTypes)
                        {
                            if (!samples.ContainsKey(mediaType))
                            {
                                // If no sample found, try generate sample using formatter and sample object
                                if (sampleResponse != null)
                                {
                                    //for request
                                    var sampleRequestObject = new ObjectFormatter().WriteSampleObjectUsingFormatter(formatter, sampleRequest, requestType, mediaType);
                                    //samples.Add(mediaType, ObjectFormatter.WrapSampleIfString(sampleRequestObject));
                                    //for response
                                    var sampleResponseObject = new ObjectFormatter().WriteSampleObjectUsingFormatter(formatter, sampleResponse, responseType, mediaType);
                                    //samples.Add(mediaType, ObjectFormatter.WrapSampleIfString(sampleResponseObject));
                                    if (mediaType.MediaType == "text/json")
                                    {
                                        _operation.OperationRequest = sampleRequestObject.ToString();
                                        _operation.OperationResponse = sampleResponseObject.ToString();
                                    }
                                }
                            }
                        }
                    }
                    arrayOfOperations.Add(_operation);
                }
                service.Operations = new List<ServiceOperations>();
                service.Operations.AddRange(arrayOfOperations);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            return service;
        }

        public void GetForSwagger() { }
    }
}