using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiddlewareApiProxy.Models
{
    public class GetServiceDescriptionRequest
    {
        public string URL { get; set; }
        public string ServiceDocs { get; set; }
        public ServicDescriptionFormat Format { get; set; }
    }

    public enum ServicDescriptionFormat
    {
        SoapXml = 0,
        SoapUrl = 1,
        SwaggerUrl = 2,
        SwaggerYaml = 3
    }

    public class GenerateServiceDescriptionRequest
    {
        public string InstitutionCode { get; set; }
        public string URL { get; set; }
        public string ServiceDocs { get; set; }
        public ServicDescriptionFormat Format { get; set; }
        public List<string> Operations { get; set; }
        public bool ForceGeneration { get; set; }
        public string ConnectorName { get; set; }
    }
}