using AppZoneMiddleware.Shared.Contracts;
using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Ionic.Crc;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using AppZoneMiddleware.Shared.Extension;
using AppZoneMiddleware.Shared.Entities.NairaBox;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Cryptography;
using System.Web.Services.Description;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Net;
using System.ServiceModel.Description;
using System.Runtime.Serialization;
using System.ServiceModel;
using Blend.SharedServiceImplementation.Services;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Collections.ObjectModel;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using WooCommerceNET;
using WooCommerceNET.WooCommerce.v3;

namespace AppzoneMiddleware.Test
{
    public class Program
    {
        public class ScriptHost
        {
            public IDictionary<string, object> sourceData { get; set; }
            public List<VerifySingleBVN> ExtData { get; set; }
        }
        public class VerifySingleBVN
        {
            public string BVN { get; set; }
            public int BankCode { get; set; }
            public List<string> cods { get; set; }
        }


        static int longestPalSubstr(String str)
        {
            int LongestPalindrome = 0;
            int CurrentPalindrome = 0;
            List<int> l = new List<int>();
            int i = 0;
            int s = 0;
            int e = 0;

            //Loop throught the sequence of characters
            while (i < str.Length)
            {

                if (i > CurrentPalindrome && str[i - CurrentPalindrome - 1] == str[i])
                {
                    CurrentPalindrome += 2;
                    i += 1;
                    continue;
                }
                l.Add(CurrentPalindrome);
                LongestPalindrome = Math.Max(LongestPalindrome, CurrentPalindrome);
                s = l.Count - 2;
                e = s - CurrentPalindrome;

                bool isCombination = false;
                for (int j = s; j > e; j--)
                {
                    int d = j - e - 1;
                    if (l[j] == d)
                    {
                        CurrentPalindrome = d;
                        // set combination to true as the smallest possible palindrome has been located
                        isCombination = true;
                        break;
                    }
                    l.Add(Math.Min(d, l[j]));
                }

                //Check if Palindrome combo has been gotten
                if (!isCombination)
                {
                    CurrentPalindrome = 1;
                    i += 1;
                }
            }
            l.Add(CurrentPalindrome);

            //Verify the longest palindrome
            LongestPalindrome = Math.Max(LongestPalindrome, CurrentPalindrome);
            return LongestPalindrome;
        }
        /**
         * DO NOT MODIFY THIS METHOD!
         */
        static void Main(String[] args)
        {
            string institutionCode = "93";
            JArray arrayOfNodes = new JArray();
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("http://dev.trublend.cloud:14000/EntityApi/api/query/{0}/0_Node", institutionCode)), Method = HttpMethod.Get })
                {
                    HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                    string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    arrayOfNodes = JsonConvert.DeserializeObject<JArray>(rawResponse);
                }

                foreach (var node in arrayOfNodes)
                {
                    var OBJ = new { InstitutionCode = institutionCode, Service = "0", NodeID = Convert.ToString(node["::ID::"]), Script = Convert.ToString(node["Request Script"]), IsRequest = true };
                    using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri("http://dev.trublend.cloud:14000/WorkflowApi/api/msgbrokr/Compilation/SaveNodeScript/12237D6C-2AE5-43A9-B599-392C42A613DA"), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(OBJ), Encoding.UTF8, "application/json") })
                    {
                        HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                        string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }
                foreach (var node in arrayOfNodes)
                {
                    var OBJ = new { InstitutionCode = institutionCode, Service = "0", NodeID = Convert.ToString(node["::ID::"]), Script = Convert.ToString(node["Response Script"]), IsRequest = false };
                    using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri("http://dev.trublend.cloud:14000/WorkflowApi/api/msgbrokr/Compilation/SaveNodeScript/12237D6C-2AE5-43A9-B599-392C42A613DA"), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(OBJ), Encoding.UTF8, "application/json") })
                    {
                        HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                        string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }

                using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri(string.Format("http://dev.trublend.cloud:14000/EntityApi/api/query/{0}/0_Node Type", institutionCode)), Method = HttpMethod.Get })
                {
                    HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                    string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    arrayOfNodes = JsonConvert.DeserializeObject<JArray>(rawResponse);
                }
                foreach (var node in arrayOfNodes)
                {
                    var OBJ = new { InstitutionCode = institutionCode, Service = "0", NodeTypeID = Convert.ToString(node["::ID::"]), Script = Convert.ToString(node["Script"]) };
                    using (HttpRequestMessage request = new HttpRequestMessage() { RequestUri = new Uri("http://dev.trublend.cloud:14000/WorkflowApi/api/msgbrokr/Compilation/SaveNodeTypeScript/12237D6C-2AE5-43A9-B599-392C42A613DA"), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(OBJ), Encoding.UTF8, "application/json") })
                    {
                        HttpResponseMessage httpResponse = client.SendAsync(request).Result;
                        string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    }
                }

            }

            RestAPI api = new RestAPI("https://www.ariiyatickets.com/dev/wp-json/wc/v3/orders", "ck_1657317ee5a49374a6ae214f4d5902013c992bc9", "cs_7685009f05fd6f19beb17c04ecb9dbaf50996a9a", true);
            WCObject wc = new WCObject(api);

            ////Get all products
            Order order = new Order()
            {
                payment_method = "payment name",
                payment_method_title = "Test Payment from partner",
                set_paid = true,
                billing = new WooCommerceNET.WooCommerce.v2.OrderBilling
                {
                    first_name = "Obi",
                    last_name = "Ade",
                    email = "ikeyy2000@gmail.com",
                    phone = "(111) 111-1111",
                },
                line_items = new List<WooCommerceNET.WooCommerce.v2.OrderLineItem>
                {
                    new WooCommerceNET.WooCommerce.v2.OrderLineItem{ product_id = 26918, quantity = 1},
                    new WooCommerceNET.WooCommerce.v2.OrderLineItem{ product_id = 25518, quantity = 1}
                },
            };
            var products = wc.Order.Add(order);

            //string twigResponse = "ERROR 0200,FIELD39=22";
            //Array twigArra = twigResponse.Split(new Char[] { ','}, StringSplitOptions.RemoveEmptyEntries);
            string val = File.ReadAllText(@"C:\Users\sosan\source\repos\Dejavu Orleans\RequestScript.txt");
            args = new string[] { "hbc_HBCM_Conet", "hBnG?_M06_t35t", "35vfbVQcZC+JaaFXWFLs3sR38uUoxqgOeknYJtOaOHcM3OlAv2xznH+LkbJUQU2AFTX9JPy8cQb3/XnjYgJ0FQP7RzXAf0DIAw7/Tr9WEx/yTAi3JQ2kJPRHoZiPPHX4" };
            JObject obj = new JObject();

            var jsonMsg = new { Body = new { input = JsonConvert.SerializeObject(new { CustomerId = "45592" }) } };// "{}";
            JObject jsonTest = new JObject();
            jsonTest.Add("body", JToken.FromObject(jsonMsg));
            ////Get a WSDL file describing a service.
            var unit = GenerateCSCodeForService(@"C:\Users\sosan\OneDrive\Documents\ProvidusPrimeMiddleware.wsdl");
            ReturnOperationsParameters(@"C:\Users\sosan\OneDrive\Documents\BlendOmniInterface.ASMX");
            //System.ServiceModel.Description.		jsonVal	null	string



            //System.Web.Services.Description.ServiceDescription description = System.Web.Services.Description.ServiceDescription.Read("C:\\Users\\Wale\\Desktop\\NFPOutwardService.wsdl");

            //// Initialize a service description importer.
            //ServiceContractGenerator generator = new ServiceContractGenerator();
            //generator.Options = ServiceContractGenerationOptions.ClientClass;




            //ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            //importer.ProtocolName = "Soap12";  // Use SOAP 1.2.
            //importer.AddServiceDescription(description, null, null);

            //// Report on the service descriptions.
            //Console.WriteLine("Importing {0} service descriptions with {1} associated schemas.",
            //                  importer.ServiceDescriptions.Count, importer.Schemas.Count);

            //// Generate a proxy client.
            //importer.Style = ServiceDescriptionImportStyle.Client;

            //// Generate properties to represent primitive values.
            //importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            //// Initialize a Code-DOM tree into which we will import the service.
            //CodeNamespace nmspace = new CodeNamespace();
            //CodeCompileUnit unit = new CodeCompileUnit();
            //unit.Namespaces.Add(nmspace);

            //// Import the service into the Code-DOM tree. This creates proxy code
            //// that uses the service.
            //ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);

            //  if (warning == 0) // If zero then we are good to go
            {

                ////Generate the proxy code
                CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                //Compile the assembly proxy with the appropriate references
                var assemblyReferences = new CompilerParameters(new[] { "System.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll", "System.Runtime.Serialization.dll", "System.ServiceModel.dll" });
                assemblyReferences.GenerateExecutable = false;
                //string[] assemblyReferences = new string[6] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll", "System.Runtime.Serialization.dll" };

                //assemblyReferences.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);
                //CompilerParameters parms = new CompilerParameters(assemblyReferences);
                //assemblyReferences.ReferencedAssemblies.Add(typeof(System.ServiceModel.EndpointAddress).Assembly.Location);
                CompilerResults results = provider1.CompileAssemblyFromDom(assemblyReferences, unit);

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

                //Finally, Invoke the web service method


                var or = results.CompiledAssembly;

                try
                {
                    object wsvcClass = results.CompiledAssembly.CreateInstance("ProvidusPrimeMiddlewareSoapClient", false, BindingFlags.Default, null, new object[]{
                   new BasicHttpBinding(),
                   new EndpointAddress("http://154.113.16.138:81/ProvidusPrimeMiddleware.asmx")
                }, null, null);
                    MethodInfo mi = wsvcClass.GetType().GetMethod("ValidateCustomerID");
                    var methodsList = wsvcClass.GetType().GetMethods();
                    var paramss = mi.GetParameters().LastOrDefault().Member;
                    var responseType = mi.ReturnParameter.ParameterType;
                    var requestType = mi.GetParameters()[0].ParameterType;
                    ObjectGenerator objectGenerator = new ObjectGenerator();
                    var sampleResponse = objectGenerator.GenerateObject(requestType);
                    ObjectPropertyGenerator objectifier = new ObjectPropertyGenerator();
                    var verifyBvnSample = objectifier.GenerateObject(requestType);
                    Collection<MediaTypeFormatter> formatters;
                    formatters = new Collection<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
                    var samples = new Dictionary<MediaTypeHeaderValue, object>();

                    foreach (var formatter in formatters)
                    {
                        foreach (MediaTypeHeaderValue mediaType in formatter.SupportedMediaTypes)
                        {
                            if (!samples.ContainsKey(mediaType))
                            {
                                // If no sample found, try generate sample using formatter and sample object
                                if (sampleResponse != null)
                                {
                                    var sample = new ObjectFormatter().WriteSampleObjectUsingFormatter(formatter, sampleResponse, requestType, mediaType);
                                    samples.Add(mediaType, ObjectFormatter.WrapSampleIfString(sample));
                                    if (mediaType.MediaType == "text/json")
                                    {

                                    }
                                }
                            }
                        }
                    }

                    var request = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(jsonTest), requestType);
                    var paramz = mi.GetParameters().FirstOrDefault().ParameterType.GetFields();
                    JArray SoapObjectRequest = new JArray();
                    foreach (var item in paramz)
                    {
                        JObject objType = new JObject { { "Name", item.Name }, { "ParameterType", item.FieldType.Name } };
                        SoapObjectRequest.Add(objType);
                    }

                    object rtn = mi.Invoke(wsvcClass, new object[] { request });
                    Type myType = rtn.GetType();
                    FieldInfo[] props = myType.GetFields();
                    string stringResponse = string.Empty;
                    foreach (FieldInfo prop in props)
                    {
                        object propValue = prop.GetValue(rtn);
                        stringResponse = JsonConvert.SerializeObject(propValue);
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex); ;
                }

            }


            //else
            //{

            //}
            //ApiProxyRequest apiRawResponse = JsonConvert.DeserializeObject<ApiProxyRequest>(body);
            //Dictionary<string, object> objectResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(apiRawResponse);
            //JObject statementResponse = JsonConvert.DeserializeObject<JObject>(Convert.ToString(objectResponse["StatusResponse"]));
            //Dictionary<string, object> response = JsonConvert.DeserializeObject<Dictionary<string, object>>(Convert.ToString(statementResponse["Response"]));
            //JObject[] statement = JsonConvert.DeserializeObject<JObject[]>(Convert.ToString(statementResponse["Transaction"]));
            string dataHeader = JsonConvert.SerializeObject(new PayLoadDataHeader[] { new PayLoadDataHeader { Key = "Authorizaation", Value = "Bearer ibnvoinwoinvoienvonwobvnoqw" } });
            string vnew = JsonConvert.SerializeObject(new JObject { new JProperty("Customer", new JArray { new JObject { new JProperty("paymentCode", "90102"), new JProperty("customerId", "07088122707") } }) });
            string USINGS = "using Newtonsoft.Json; using System;using System.IO;using System.Xml.Serialization; using System.Xml.Linq;using System.Xml;using System.Linq;using System.Threading.Tasks;using System.Collections;using System.Collections.Generic;using System.Text;using System.Dynamic;\r\n";

            //var dic = new Dictionary<string, object> { { "Header", new Dictionary<string, object> { { "RequestHeader", new Dictionary<string, object> { { "MessageKey", MessageKey } } } } }, { "Body", new Dictionary<string, object> { { "RetCustAddRequest", RetCustAddRequest } } } };
            var sourceData = new Dictionary<string, object> { { "RequestUUID", "Req_1426162415487" }, { "ServiceRequestId", "RetCustAdd" }, { "ServiceRequestVersion", "10.2" }, { "AddrLine1", "01" }, { "BirthDt", "31" }, { "Language", "UK (English)" }, { "LastName", "Clement" }, { "MiddleName", "Nosariemen" }, { "DocCode", "PSPRT" } };

            string json = JsonConvert.SerializeObject(new JObject { { "myUsername", "hbc_HBCM_Conet" }, { "myPswd", "hBnG?_M06_t35t" }, { "LocalNameEnquiryRequest", "{\"uniqueIdentifier\":\"{\"16738027848\",\"AccountNumber\":\"1913446553\"}\"}" } });
            JObject pubg = JsonConvert.DeserializeObject<JObject>(json);
            //" \"data\":{\"BVN\":\"2170077974\", \"BankCode\":\"23\"}";

            //FIXML xmlGuy = new FIXMLHelper().InitializeFI(sourceData);
            //string qualityXML = Serialize(xmlGuy);
            //var str2 = xmlGuy.ToXmlString();

            obj = JsonConvert.DeserializeObject<JObject>(val);
            string stringfy = JsonConvert.SerializeObject(obj);

            // you get the base64 encoded key from somewhere
            var base64Key = "2344asdfWPOuARSFUs10Lm==";
            // convert it to byte[] or alternatively you could store your key as a byte[] 
            //   but that depends on how you set things up.
            var encryptedText = EncryptString(stringfy, base64Key);
            var decryptedText = DecryptString(encryptedText, base64Key);
            //XNamespace ns = "http://www.finacle.com/fixml";
            //XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            //XElement fiXElment = new XElement("FIXLM",
            //    new XAttribute(xsi + "Schemalocation", "http://www.finacle.com/fixml RetCustAdd.xsd"),
            //    new XAttribute("Xmlns", ns.NamespaceName),
            //    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
            //XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null), fiXElment);
            //var sb = new StringBuilder();
            //using (var sw = new StringWriterUtf8(sb))
            //{
            //    doc.Save(sw);
            //    Console.WriteLine(sb.ToString());
            //};


            //XNamespace ns = "http://www.finacle.com/fixml";
            //XNamespace xsi = "http://www.finacle.com/fixml";
            //XElement fiXElment = new XElement(new XElement("FIXLM",
            //    new XAttribute(xsi + "Schemalocation", "http://www.finacle.com/fixml RetCustAdd.xsd"),
            //    new XAttribute("Xmlns", ns.NamespaceName),
            //    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            //    new FIXMLHelper().GenerateFiXMlHeader(sourceData), new FIXMLHelper().GenerateFIXMLBody(sourceData)));
            //XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null), fiXElment);
            //var sb = new StringBuilder();
            //using (var sw = new StringWriterUtf8(sb))
            //{
            //    doc.Save(sw);
            //    //Console.WriteLine(sb.ToString());
            //};


            ////Encoding encoding = Encoding.UTF8;
            ////XmlDocument doc = new XmlDocument();
            ////XmlDeclaration declarationNode = (XmlDeclaration)doc.CreateNode(XmlNodeType.XmlDeclaration, "xml", string.Empty);
            ////declarationNode.Encoding = encoding.HeaderName;
            ////doc.InsertBefore(declarationNode, doc.FirstChild);



            //var script = File.ReadAllText("C:\\Users\\Wale\\Desktop\\ConversionScript.cs");
            //CompilationReferences.LoadReferences();
            //var scriptOptions = ScriptOptions.Default
            //              .AddReferences(CompilationReferences.CommandReferences);
            //StringBuilder stb = new StringBuilder();
            //stb.AppendLine(USINGS);
            //stb.AppendLine(script);

            ////note: we block here, because we are in Main method, normally we could await as scripting APIs are async
            //var result = CSharpScript.EvaluateAsync<string>(stb.ToString(), scriptOptions, new ScriptHost { sourceData = sourceData }).Result;

            //var response = File.ReadAllText("C:\\Users\\Wale\\Desktop\\CIFCreationResponse.xml");

            ////result is now 5
            //Console.WriteLine(result);
            ////Console.ReadLine();
            //XNamespace ns = "http://www.finacle.com/fixml";
            //XDocument xdoc = XDocument.Parse(response);
            //var fixml = xdoc.Root;
            //var value = new
            //{
            //    Status = fixml.Element(ns + "Header").Element(ns + "ResponseHeader").Element(ns + "HostTransaction").Element(ns + "Status").Value,
            //    CustomerID = fixml.Element(ns + "Body").Element(ns + "RetCustAddResponse").Element(ns + "RetCustAddRs").Element(ns + "CustId").Value,
            //    ResponseMessage = fixml.Element(ns + "Body").Element(ns + "RetCustAddResponse").Element(ns + "RetCustAddRs").Element(ns + "Desc").Value,
            //};

            //Console.WriteLine(value);
            var logData = new Dictionary<string, string> { { "Response Code", "06" }, { "Response Message", "Issue with Node/Node~Type setup" } };
            Task.Run(() => new ElasticSearchLogger().LogMessage("Main Method", logData, "2424"));
            Console.ReadKey();
        }

        public static string ExtractJSON(string src)
        {
            src = src.Replace("\r\n", string.Empty)
                         .Replace("<Response>", string.Empty)
                         .Replace("</Response>", string.Empty)
                         .Trim('"')
                         .Replace("\\\"\\\"", "\" \"")  // Fix for a JSON Serialization Exception. Note that the space in-between is intentional, and it's for properties that are NULL!
                         .Replace("\\\"", "\"")
                         .Replace("\"{", "{")
                         .Replace("}\"", "}")
                         .Replace("\"[", "[")
                         .Replace("]\"", "]")
                         .Replace("\\{", "{")
                         .Replace("}\\", "}")
                         .Replace("\\[", "[")
                         .Replace("]\\", "]");
            //.Replace("\\", string.Empty).Replace("\\\\", string.Empty).Replace("\"\"", "\"");

            if (src.Contains("\\\""))
            {
                src = ExtractJSON(src);
            }

            src = System.Net.WebUtility.HtmlDecode(src);

            return src;
        }

        public static void ReturnOperationsParameters(string fileName)
        {

            var reader = new XmlTextReader(fileName);
            var serviceDescription = System.Web.Services.Description.ServiceDescription.Read(reader);
            BindingCollection bindColl = serviceDescription.Bindings;
            PortTypeCollection portTypColl = serviceDescription.PortTypes;
            MessageCollection msgColl = serviceDescription.Messages;
            Types typs = serviceDescription.Types;


            foreach (Service service in serviceDescription.Services)
            {
                String webServiceNmae = service.Name.ToString();

                foreach (Port port in service.Ports)
                {
                    string portName = port.Name;
                    string binding = port.Binding.Name;
                    System.Web.Services.Description.Binding bind = bindColl[binding];
                    PortType portTyp = portTypColl[bind.Type.Name];
                    foreach (Operation op in portTyp.Operations)
                    {
                        var operatioList = new SoapData();
                        // _soapdata = new SoapData();
                        OperationMessageCollection opMsgColl = op.Messages;
                        OperationInput opInput = opMsgColl.Input;
                        string inputMsg = opInput.Message.Name;
                        Message msgInput = msgColl[inputMsg];
                        MessagePart part = msgInput.Parts[0];
                        operatioList.OperationName = op.Name;


                        operatioList.NameSpace = part.Element.Namespace;
                    }
                }
            }

        }

        public class SoapData
        {
            public int Id { get; set; }
            public string RequestXml { get; set; }
            public string ResponseXml { get; set; }
            public string NameSpace { get; set; }
            public string OperationName { get; set; }
        }



        public static Collection<MediaTypeFormatter> SetForatters(string[] configuredTypes)
        {
            Collection<MediaTypeFormatter> collection = new Collection<MediaTypeFormatter>();
            MediaTypeFormatter format;
            foreach (var item in configuredTypes)
            {
                switch (item.ToLower())
                {
                    case "application/json":
                        collection.Add(new JsonMediaTypeFormatter());
                        break;
                    case "application/xml":
                        collection.Add(new XmlMediaTypeFormatter());
                        break;
                    default:
                        //throw new UnsupportedMediaTypeException("Media type not supported", new MediaTypeHeaderValue(item));
                        break;
                }
            }
            return collection;
        }


        #region Service Description Importer
        static void Main_oo(string[] args)
        {
            List<int> a = new List<int>();
            a.Add(1);
            List<int> bb = new List<int>();
            bb.Add(1);
            Console.WriteLine(a == bb);
            args = new string[] { "hbc_HBCM_Conet", "hBnG?_M06_t35t", "35vfbVQcZC+JaaFXWFLs3sR38uUoxqgOeknYJtOaOHcM3OlAv2xznH+LkbJUQU2AFTX9JPy8cQb3/XnjYgJ0FQP7RzXAf0DIAw7/Tr9WEx/yTAi3JQ2kJPRHoZiPPHX4" };
            JObject obj = new JObject();
            obj.Add("OperationName", "");
            obj.Add("MethodName", "");
            obj.Add("args", JsonConvert.SerializeObject(args));
            string x = JsonConvert.SerializeObject(obj);
            //Get a WSDL file describing a service.
            System.Web.Services.Description.ServiceDescription description = System.Web.Services.Description.ServiceDescription.Read("C:\\Users\\Wale\\Desktop\\NFPOutwardService.wsdl");

            // Initialize a service description importer.
            ServiceDescriptionImporter importer = new ServiceDescriptionImporter();
            importer.ProtocolName = "Soap12";  // Use SOAP 1.2.
            importer.AddServiceDescription(description, null, null);

            // Report on the service descriptions.
            Console.WriteLine("Importing {0} service descriptions with {1} associated schemas.",
                              importer.ServiceDescriptions.Count, importer.Schemas.Count);

            // Generate a proxy client.
            importer.Style = ServiceDescriptionImportStyle.Client;

            // Generate properties to represent primitive values.
            importer.CodeGenerationOptions = System.Xml.Serialization.CodeGenerationOptions.GenerateProperties;

            // Initialize a Code-DOM tree into which we will import the service.
            CodeNamespace nmspace = new CodeNamespace();
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(nmspace);

            // Import the service into the Code-DOM tree. This creates proxy code
            // that uses the service.
            ServiceDescriptionImportWarnings warning = importer.Import(nmspace, unit);

            if (warning == 0) // If zero then we are good to go
            {

                // Generate the proxy code
                CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                // Compile the assembly proxy with the appropriate references
                string[] assemblyReferences = new string[5] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll" };

                CompilerParameters parms = new CompilerParameters(assemblyReferences);

                CompilerResults results = provider1.CompileAssemblyFromDom(parms, unit);

                // Check For Errors
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError oops in results.Errors)
                    {
                        System.Diagnostics.Debug.WriteLine("========Compiler error============");
                        System.Diagnostics.Debug.WriteLine(oops.ErrorText);
                    }
                    throw new System.Exception("Compile Error Occured calling webservice. Check Debug ouput window.");
                }

                // Finally, Invoke the web service method

                object wsvcClass = results.CompiledAssembly.CreateInstance("NFPOutwardService");

                MethodInfo mi = wsvcClass.GetType().GetMethod("nameenquirysingleitem");
                var propInfo = wsvcClass.GetType().GetRuntimeProperty("AuthHeaderValue").PropertyType;
                var toObj = wsvcClass.GetType().GetRuntimeProperty("AuthHeaderValue");
                //var propInfo = mi.GetCustomAttribute(typeof(System.Web.Services.Protocols.SoapHeaderAttribute));

                var met = wsvcClass.GetType().GetRuntimeProperty("AuthHeaderValue").SetMethod;

                //foreach (var prop in propInfo.GetProperties())
                //{
                //    if (prop.Name == "UserName")
                //    {
                //        MethodInfo meth = prop.SetMethod;
                //        meth.Invoke(prop, new string[] { "username" });
                //        prop.SetValue(toObj, "username", null);
                //    }
                //    else if (prop.Name == "Password")
                //    {
                //        prop.SetValue(wsvcClass, "pword", null);
                //    }
                //}
                //if (prop.Name == "UserName")
                //{
                //prop.SetValue(mi, "username", null);
                //}
                //else if (prop.Name == "Password")
                //{
                //prop.SetValue(mi, "pword", null);
                //}


                object rtn = mi.Invoke(wsvcClass, args);

            }

            else
            {

            }

            string USINGS = "using Newtonsoft.Json; using System;using System.IO;using System.Xml.Serialization; using System.Xml.Linq;using System.Xml;using System.Linq;using System.Threading.Tasks;using System.Collections;using System.Collections.Generic;using System.Text;using System.Dynamic;\r\n";

            //var dic = new Dictionary<string, object> { { "Header", new Dictionary<string, object> { { "RequestHeader", new Dictionary<string, object> { { "MessageKey", MessageKey } } } } }, { "Body", new Dictionary<string, object> { { "RetCustAddRequest", RetCustAddRequest } } } };
            var sourceData = new Dictionary<string, object> { { "RequestUUID", "Req_1426162415487" }, { "ServiceRequestId", "RetCustAdd" }, { "ServiceRequestVersion", "10.2" }, { "AddrLine1", "01" }, { "BirthDt", "31" }, { "Language", "UK (English)" }, { "LastName", "Clement" }, { "MiddleName", "Nosariemen" }, { "DocCode", "PSPRT" } };

            string json = JsonConvert.SerializeObject(new JObject { { "myUsername", "hbc_HBCM_Conet" }, { "myPswd", "hBnG?_M06_t35t" }, { "LocalNameEnquiryRequest", "{\"uniqueIdentifier\":\"{\"16738027848\",\"AccountNumber\":\"1913446553\"}\"}" } });
            JObject pubg = JsonConvert.DeserializeObject<JObject>(json);
            //" \"data\":{\"BVN\":\"2170077974\", \"BankCode\":\"23\"}";

            //FIXML xmlGuy = new FIXMLHelper().InitializeFI(sourceData);
            //string qualityXML = Serialize(xmlGuy);
            //var str2 = xmlGuy.ToXmlString();

            obj = new JObject { { "nAE", "iwnvwi" }, { "pass", "wunviowvo" }, { "dtat", "{\"BVN\":\"2170077974\", \"BankCode\":\"23\"}" } };
            string stringfy = JsonConvert.SerializeObject(obj);

            // you get the base64 encoded key from somewhere
            var base64Key = "2344asdfWPOuARSFUs10Lm==";
            // convert it to byte[] or alternatively you could store your key as a byte[] 
            //   but that depends on how you set things up.
            var encryptedText = EncryptString(File.ReadAllText("C:\\Users\\Wale\\Desktop\\TestJson.json"), base64Key);
            var decryptedText = DecryptString(encryptedText, base64Key);

            //XNamespace ns = "http://www.finacle.com/fixml";
            //XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            //XElement fiXElment = new XElement("FIXLM",
            //    new XAttribute(xsi + "Schemalocation", "http://www.finacle.com/fixml RetCustAdd.xsd"),
            //    new XAttribute("Xmlns", ns.NamespaceName),
            //    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"));
            //XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null), fiXElment);
            //var sb = new StringBuilder();
            //using (var sw = new StringWriterUtf8(sb))
            //{
            //    doc.Save(sw);
            //    Console.WriteLine(sb.ToString());
            //};


            //XNamespace ns = "http://www.finacle.com/fixml";
            //XNamespace xsi = "http://www.finacle.com/fixml";
            //XElement fiXElment = new XElement(new XElement("FIXLM",
            //    new XAttribute(xsi + "Schemalocation", "http://www.finacle.com/fixml RetCustAdd.xsd"),
            //    new XAttribute("Xmlns", ns.NamespaceName),
            //    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            //    new FIXMLHelper().GenerateFiXMlHeader(sourceData), new FIXMLHelper().GenerateFIXMLBody(sourceData)));
            //XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null), fiXElment);
            //var sb = new StringBuilder();
            //using (var sw = new StringWriterUtf8(sb))
            //{
            //    doc.Save(sw);
            //    //Console.WriteLine(sb.ToString());
            //};


            ////Encoding encoding = Encoding.UTF8;
            ////XmlDocument doc = new XmlDocument();
            ////XmlDeclaration declarationNode = (XmlDeclaration)doc.CreateNode(XmlNodeType.XmlDeclaration, "xml", string.Empty);
            ////declarationNode.Encoding = encoding.HeaderName;
            ////doc.InsertBefore(declarationNode, doc.FirstChild);



            //var script = File.ReadAllText("C:\\Users\\Wale\\Desktop\\ConversionScript.cs");
            //CompilationReferences.LoadReferences();
            //var scriptOptions = ScriptOptions.Default
            //              .AddReferences(CompilationReferences.CommandReferences);
            //StringBuilder stb = new StringBuilder();
            //stb.AppendLine(USINGS);
            //stb.AppendLine(script);

            ////note: we block here, because we are in Main method, normally we could await as scripting APIs are async
            //var result = CSharpScript.EvaluateAsync<string>(stb.ToString(), scriptOptions, new ScriptHost { sourceData = sourceData }).Result;

            //var response = File.ReadAllText("C:\\Users\\Wale\\Desktop\\CIFCreationResponse.xml");

            ////result is now 5
            //Console.WriteLine(result);
            ////Console.ReadLine();
            //XNamespace ns = "http://www.finacle.com/fixml";
            //XDocument xdoc = XDocument.Parse(response);
            //var fixml = xdoc.Root;
            //var value = new
            //{
            //    Status = fixml.Element(ns + "Header").Element(ns + "ResponseHeader").Element(ns + "HostTransaction").Element(ns + "Status").Value,
            //    CustomerID = fixml.Element(ns + "Body").Element(ns + "RetCustAddResponse").Element(ns + "RetCustAddRs").Element(ns + "CustId").Value,
            //    ResponseMessage = fixml.Element(ns + "Body").Element(ns + "RetCustAddResponse").Element(ns + "RetCustAddRs").Element(ns + "Desc").Value,
            //};

            //Console.WriteLine(value);
            var logData = new Dictionary<string, string> { { "Response Code", "06" }, { "Response Message", "Issue with Node/Node~Type setup" } };
            Task.Run(() => new ElasticSearchLogger().LogMessage("Main Method", logData, "2424"));
            Console.ReadKey();
        }

        #endregion

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

            var or = description.Services[0];

            foreach (ContractDescription contract in contracts)
            {
                generator.GenerateServiceContractType(contract);
            }
            if (generator.Errors.Count != 0)
                throw new Exception("There were errors during code compilation.");

            // Write the code dom
            //CodeGeneratorOptions options = new CodeGeneratorOptions() { BracingStyle = "C" };
            //CodeDomProvider codeDomProvider = CodeDomProvider.CreateProvider("C#");
            //IndentedTextWriter textWriter = new IndentedTextWriter(new StreamWriter("jidecs.cs"));
            //codeDomProvider.GenerateCodeFromCompileUnit(
            //  generator.TargetCompileUnit, textWriter, options
            //);
            return generator.TargetCompileUnit;
        }

        public static void SerilogGraylogUtilLogger()
        {
            Console.ReadLine();
            //    GrayLogUtil.LoggerInstance.Information("SerilogGraylogUtilLogger => Hello, world!");

            //    int a = 10, b = 0;
            //    try
            //    {
            //        GrayLogUtil.LoggerInstance.Debug("Dividing {A} by {B}", a, b);
            //        Console.WriteLine(a / b);
            //    }
            //    catch (Exception ex)
            //    {
            //        GrayLogUtil.LoggerInstance.Error(ex, "Something went wrong");
            //    }

            //Log.CloseAndFlush();
        }

        static string EncryptString(string plainText, string Key)
        {
            string encryptedString = string.Empty;
            try
            {
                var aes = new AesManaged();
                aes.Key = Convert.FromBase64String(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptor = aes.CreateEncryptor();
                var plainTextByte = Encoding.UTF8.GetBytes(plainText);
                var cypher = cryptor.TransformFinalBlock(plainTextByte, 0, plainTextByte.Length);
                encryptedString = Convert.ToBase64String(cypher);
            }
            catch (Exception)
            {

                throw;
            }
            return encryptedString;
        }

        static string DecryptString(string base64CipherText, string Key)
        {
            string decryptedString = string.Empty;
            try
            {
                var plainCypherText = Convert.FromBase64String(base64CipherText);
                var aes = new AesManaged();
                aes.Key = Convert.FromBase64String(Key);
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptor = aes.CreateDecryptor();
                var cypher = cryptor.TransformFinalBlock(plainCypherText, 0, plainCypherText.Length);
                decryptedString = Encoding.UTF8.GetString(cypher);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decryptedString;
        }

        private static String UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public class HourlyShareRateModel
        {
            public static int Id { get; set; }

            [Required]
            public DateTime TimeStamp { get; set; }

            [Required]
            [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Share symbol should be all capital letters with 3 characters")]
            public string Symbol { get; set; }

            [Required]
            public decimal Rate { get; set; }
        }

    }
}
