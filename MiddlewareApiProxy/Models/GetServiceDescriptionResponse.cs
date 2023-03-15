using AppZoneMiddleware.Shared.Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiddlewareApiProxy.Models
{
    public class GetServiceDescriptionResponse
    {
        public string ContractName { get; set; }
        public List<ServiceOperations> Operations { get; set; }
        public string ClientName { get; set; }
    }

    public class ServiceOperations
    {
        public string OperationName { get; set; }
        public string OperationRequest { get; set; }
        public string OperationResponse { get; set; }
        public Dictionary<string, object> RequestMessageProperties { get; set; }
        public Dictionary<string, object> ResponseMessageProperties { get; set; }
    }

    public class GenerateOrleansOperationReq
    {
        public string InstitutionCode { get; set; }
        public string URL { get; set; }
        public string ClientName { get; set; }
        public string ContractName { get; set; }
        public bool ForceGeneration { get; set; }
        public string ConnectorName { get; set; }
        public List<ServiceOperations> Operations { get; set; }
    }

    public class GenerateOperationsResponse : BaseResponse
    {
        public JObject GeneratedOperations { get; set; }
    }
}
