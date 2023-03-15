using AppZoneMiddleware.Shared.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AppzoneMiddleware.Test
{
    class TestingGenerics<G, T>
    {
        public object Processor(G RequestMessage)
        {
            StringBuilder RequestXml = new StringBuilder();
            var customerSerializer = new XmlSerializer(typeof(G));
            using (var writer = new StringWriter(RequestXml))
            {
                customerSerializer.Serialize(writer, RequestMessage);
                Logger.LogInfo("AccountInquiryService.BalanceEnquiry", String.Format("Balance Inquiry Request: {0}", RequestXml));
            }

            string customerResponse = File.ReadAllText("C:\\Users\\RONNIE\\Desktop\\SterlingXMLResp.xml");
            Logger.LogInfo("AccountInquiryService.BalanceEnquiry", String.Format("Balance Inquiry Response: {0}", customerResponse));
            var customerRessponseSerializer1 = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(customerResponse))
            {
                var deserialized = customerRessponseSerializer1.Deserialize(reader);
                Logger.LogInfo("AccountInquiryService.BalanceEnquiry: result", deserialized);
                return deserialized;

            }
        }
    }
}
