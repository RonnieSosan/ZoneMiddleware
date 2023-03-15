using AppZoneMiddleware.Shared.Entities;
using AppZoneMiddleware.Shared.Utility;
using Blend.SharedServiceImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blend.SterlingImplementation.ServiceUtilities
{
    /// <summary>
    /// Process for sending messages over to the bridge and back 
    /// </summary>
    /// <typeparam name="G">Type of request class</typeparam>
    /// <typeparam name="T">Type of response class</typeparam>
    public class IBSBridgeProcessor<G, T>
    {
        int webServiceAppId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]);

        public IBSBridgeProcessor()
        {
        }

        public object Processor(G RequestMessage)
        {
            return Processor(RequestMessage, false);
        }

        public object Processor(G RequestMessage, bool StripNameSpace)
        {
            Logger.LogInfo("IBSBridgeProcessor.Processor: request", RequestMessage);
            StringBuilder RequestXml = new StringBuilder();
            var clientRequestSerializer = new XmlSerializer(typeof(G));
            using (var writer = new StringWriter(RequestXml))
            {
                if (StripNameSpace)
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    clientRequestSerializer.Serialize(writer, RequestMessage, ns);
                }
                else
                {
                    clientRequestSerializer.Serialize(writer, RequestMessage);
                }
            }
            Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format("Request Message: {0}", RequestXml));

            string serverApiResponse = "";
            using (SterlingIBService.BSServicesSoapClient client = new SterlingIBService.BSServicesSoapClient())
            {
                serverApiResponse = client.IBSBridge(new ServiceUtilities.Utilities().SterlingEncrypt(RequestXml.ToString()), webServiceAppId);
                serverApiResponse = new ServiceUtilities.Utilities().SterlingDecrypt(serverApiResponse);
            }
            Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format(" Response Message: {0}", serverApiResponse));

            var apiResponseDeserializer = new XmlSerializer(typeof(T));
            object deserialized = default(T);
            using (var reader = new StringReader(serverApiResponse))
            {
                deserialized = apiResponseDeserializer.Deserialize(reader);
            }
            Logger.LogInfo("IBSBridgeProcessor.Processor: result", deserialized);
            return deserialized;
        }

        public object ProcessIBSBridge(G RequestMessage, bool StripNameSpace)
        {
            Logger.LogInfo("IBSBridgeProcessIBSBridge.ProcessIBSBridge: request", RequestMessage);
            StringBuilder RequestXml = new StringBuilder();

            if (typeof(G).FullName == typeof(string).FullName)
            {
                RequestXml.Append(Convert.ToString(RequestMessage));
            }
            else
            {
                var clientRequestSerializer = new XmlSerializer(typeof(G));
                using (var writer = new StringWriter(RequestXml))
                {
                    if (StripNameSpace)
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");
                        clientRequestSerializer.Serialize(writer, RequestMessage, ns);
                    }
                    else
                    {
                        clientRequestSerializer.Serialize(writer, RequestMessage);
                    }
                }
            }
            Logger.LogInfo("IBSBridgeProcessIBSBridge.ProcessIBSBridge", String.Format("Request Message: {0}", RequestXml));

            string serverApiResponse = "";
            using (SterlingIBService.BSServicesSoapClient client = new SterlingIBService.BSServicesSoapClient())
            {
                serverApiResponse = client.IBSBridge(new ServiceUtilities.Utilities().SterlingEncrypt(RequestXml.ToString()), webServiceAppId);
                serverApiResponse = new ServiceUtilities.Utilities().SterlingDecrypt(serverApiResponse);
            }
            Logger.LogInfo("IBSBridgeProcessIBSBridge.ProcessIBSBridge", String.Format(" Response Message: {0}", serverApiResponse));

            object deserialized = default(T);
            if (typeof(T).FullName == typeof(string).FullName)
            {
                deserialized = serverApiResponse;
            }
            else
            {
                var apiResponseDeserializer = new XmlSerializer(typeof(T));
                using (var reader = new StringReader(serverApiResponse))
                {
                    deserialized = apiResponseDeserializer.Deserialize(reader);
                }
                Logger.LogInfo("IBSBridgeProcessIBSBridge.ProcessIBSBridge: result", deserialized);
            }
            return deserialized;
        }

        public object Processor(G RequestMessage, bool StripNameSpace, bool processCards, Encoding encoding)
        {
            Logger.LogInfo(String.Format("IBSBridgeProcessor.ProcessorForCard RequestMessage: {0} : input", RequestMessage), RequestMessage);
            StringBuilder RequestXml = new StringBuilder();
            var clientRequestSerializer = new XmlSerializer(typeof(G));
            using (var writer = new DesiredEncodingStringWriter(RequestXml, encoding))
            {
                if (StripNameSpace)
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");
                    clientRequestSerializer.Serialize(writer, RequestMessage, ns);
                }
                else
                {
                    clientRequestSerializer.Serialize(writer, RequestMessage);
                }
            }
            Logger.LogInfo("IBSBridgeProcessor.ProcessorForCard", String.Format("Request Message: {0}", RequestXml));

            string serverApiResponse = "";
            using (IBCardService.CardServClient client = new IBCardService.CardServClient())
            {
                serverApiResponse = client.CardOps(RequestXml.ToString());
                //serverApiResponse = new ServiceUtilities.Utilities().SterlingDecrypt(serverApiResponse);
            }
            Logger.LogInfo("IBSBridgeProcessor.ProcessorForCard", String.Format(" Response Message: {0}", serverApiResponse));

            var apiResponseDeserializer = new XmlSerializer(typeof(T));
            object deserialized = default(T);
            using (var reader = new StringReader(serverApiResponse))
            {
                deserialized = apiResponseDeserializer.Deserialize(reader);
            }
            Logger.LogInfo("IBSBridgeProcessor.ProcessorForCard: result", deserialized);
            return deserialized;
        }

        public bool ValidateTokenFromExternalSource(G TokenObj, out BaseResponse resp)
        {
            string response = Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Token", "TokenAuthentication").CallService<UserProfileResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(TokenObj))).Result;
            UserProfileResponse authorizationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(response);
            resp = authorizationResponse;
            // TODO: Uncomment the commented line below. Then, remove the next line.
            // return authorizationResponse.ResponseCode == "00";
            return (!string.IsNullOrWhiteSpace(authorizationResponse.ResponseDescription) && authorizationResponse.ResponseDescription.ToLowerInvariant().Contains("token".ToLowerInvariant()) ? true : authorizationResponse.ResponseCode == "00");
        }

        public bool ValidatePinFromExternalSource(G TokenObj, out BaseResponse resp)
        {
            string response = Task.Factory.StartNew(() => new AppzoneApiProcessor("Blend", "Token", "TransactionalTokenAuthentication").CallService<UserProfileResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(TokenObj))).Result;
            UserProfileResponse authorizationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(response);
            resp = authorizationResponse;
            // TODO: Uncomment the commented line below. Then, remove the next line.
            // return authorizationResponse.ResponseCode == "00";
            return (!string.IsNullOrWhiteSpace(authorizationResponse.ResponseDescription) && authorizationResponse.ResponseDescription.ToLowerInvariant().Contains("token".ToLowerInvariant()) ? true : authorizationResponse.ResponseCode == "00");
        }
    }
}
