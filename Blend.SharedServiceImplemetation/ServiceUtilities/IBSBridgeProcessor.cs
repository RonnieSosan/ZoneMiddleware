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

namespace Blend.SharedServiceImplementation.ServiceUtilities
{
    /// <summary>
    /// Process for sending messages od  to the bridge and back 
    /// </summary>
    /// <typeparam name="G">Type of request class</typeparam>
    /// <typeparam name="T">Type of response class</typeparam>
    public class IBSBridgeProcessor<G, T>
    {
        readonly int webServiceAppId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["WebServiceAppId"]);

        public object Processor(G RequestMessage)
        {
            Logger.LogInfo("IBSBridgeProcessor.Processor: result", RequestMessage);
            StringBuilder RequestXml = new StringBuilder();
            var clientRequestSerializer = new XmlSerializer(typeof(G));
            using (var writer = new StringWriter(RequestXml))
            {
                clientRequestSerializer.Serialize(writer, RequestMessage);
                Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format("Request Message: {0}", RequestXml));
            }

            string serverApiResponse = "";
            using (SharedServiceImplementation.SterlingIBService.BSServicesSoapClient client = new SharedServiceImplementation.SterlingIBService.BSServicesSoapClient())
            {
                serverApiResponse = client.IBSBridge(new ServiceUtilities.Utilities().SterlingEncrypt(RequestXml.ToString()), webServiceAppId);
                serverApiResponse = new ServiceUtilities.Utilities().SterlingDecrypt(serverApiResponse);
                Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format(" Response Message: {0}", serverApiResponse));
            }

            var apiResponseDeserializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(serverApiResponse))
            {
                var deserialized = apiResponseDeserializer.Deserialize(reader);
                Logger.LogInfo("IBSBridgeProcessor.Processor: result", deserialized);
                return deserialized;

            }
        }

        public object Processor(G RequestMessage, bool StripNameSpace)
        {
            if (!StripNameSpace)
            {
                return Processor(RequestMessage);
            }
            Logger.LogInfo("IBSBridgeProcessor.Processor: result", RequestMessage);
            StringBuilder RequestXml = new StringBuilder();
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var clientRequestSerializer = new XmlSerializer(typeof(G));
            using (var writer = new StringWriter(RequestXml))
            {
                clientRequestSerializer.Serialize(writer, RequestMessage, ns);
                Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format("Request Message: {0}", RequestXml));
            }

            string serverApiResponse = "";
            using (SharedServiceImplementation.SterlingIBService.BSServicesSoapClient client = new SharedServiceImplementation.SterlingIBService.BSServicesSoapClient())
            {
                serverApiResponse = client.IBSBridge(new ServiceUtilities.Utilities().SterlingEncrypt(RequestXml.ToString()), webServiceAppId);
                serverApiResponse = new ServiceUtilities.Utilities().SterlingDecrypt(serverApiResponse);
                Logger.LogInfo("IBSBridgeProcessor.Processor", String.Format(" Response Message: {0}", serverApiResponse));
            }

            var apiResponseDeserializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(serverApiResponse))
            {
                var deserialized = apiResponseDeserializer.Deserialize(reader);
                Logger.LogInfo("IBSBridgeProcessor.Processor: result", deserialized);
                return deserialized;

            }
        }

        public bool ValidateTokenFromExternalSource(G TokenObj)
        {
            string response = Task.Factory.StartNew(() => new ProfileApiProcessor("Blend", "Token", "TokenAuthentication").CallService<UserProfileResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(TokenObj))).Result;
            UserProfileResponse authorizationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(response);
            return authorizationResponse.ResponseCode == "00";
        }

        public bool ValidatePinFromExternalSource(G TokenObj)
        {
            string response = Task.Factory.StartNew(() => new ProfileApiProcessor("Blend", "Token", "TransactionalTokenAuthentication").CallService<UserProfileResponse>(Newtonsoft.Json.JsonConvert.SerializeObject(TokenObj))).Result;
            UserProfileResponse authorizationResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<UserProfileResponse>(response);
            return authorizationResponse.ResponseCode == "00";
        }
    }
}
