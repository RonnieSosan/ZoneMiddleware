﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blend.ProvidusImplementation.NipWrapperService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.CollectionDataContractAttribute(Name="ArrayOfString", Namespace="http://tempuri.org/", ItemName="string")]
    [System.SerializableAttribute()]
    public class ArrayOfString : System.Collections.Generic.List<string> {
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="NipWrapperService.NipWrapperServiceSoap")]
    public interface NipWrapperServiceSoap {
        
        // CODEGEN: Generating message contract since element name DestinationBankCode from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/nameenquirysingleitem", ReplyAction="*")]
        Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse nameenquirysingleitem(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/nameenquirysingleitem", ReplyAction="*")]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse> nameenquirysingleitemAsync(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest request);
        
        // CODEGEN: Generating message contract since element name GetNFPBanksResult from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetNFPBanks", ReplyAction="*")]
        Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse GetNFPBanks(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetNFPBanks", ReplyAction="*")]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse> GetNFPBanksAsync(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest request);
        
        // CODEGEN: Generating message contract since element name SessionId from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/fundtransfersingleitem_dc", ReplyAction="*")]
        Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse fundtransfersingleitem_dc(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/fundtransfersingleitem_dc", ReplyAction="*")]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse> fundtransfersingleitem_dcAsync(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest request);
        
        // CODEGEN: Generating message contract since element name Amount from namespace http://tempuri.org/ is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetNIBSSCharges", ReplyAction="*")]
        Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse GetNIBSSCharges(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetNIBSSCharges", ReplyAction="*")]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse> GetNIBSSChargesAsync(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nameenquirysingleitemRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="nameenquirysingleitem", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequestBody Body;
        
        public nameenquirysingleitemRequest() {
        }
        
        public nameenquirysingleitemRequest(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class nameenquirysingleitemRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string DestinationBankCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string AccountNo;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string ChannelCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string SourceAccountNumber;
        
        public nameenquirysingleitemRequestBody() {
        }
        
        public nameenquirysingleitemRequestBody(string DestinationBankCode, string AccountNo, string ChannelCode, string SourceAccountNumber) {
            this.DestinationBankCode = DestinationBankCode;
            this.AccountNo = AccountNo;
            this.ChannelCode = ChannelCode;
            this.SourceAccountNumber = SourceAccountNumber;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nameenquirysingleitemResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="nameenquirysingleitemResponse", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponseBody Body;
        
        public nameenquirysingleitemResponse() {
        }
        
        public nameenquirysingleitemResponse(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class nameenquirysingleitemResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string nameenquirysingleitemResult;
        
        public nameenquirysingleitemResponseBody() {
        }
        
        public nameenquirysingleitemResponseBody(string nameenquirysingleitemResult) {
            this.nameenquirysingleitemResult = nameenquirysingleitemResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetNFPBanksRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetNFPBanks", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequestBody Body;
        
        public GetNFPBanksRequest() {
        }
        
        public GetNFPBanksRequest(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute()]
    public partial class GetNFPBanksRequestBody {
        
        public GetNFPBanksRequestBody() {
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetNFPBanksResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetNFPBanksResponse", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponseBody Body;
        
        public GetNFPBanksResponse() {
        }
        
        public GetNFPBanksResponse(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetNFPBanksResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.ArrayOfString GetNFPBanksResult;
        
        public GetNFPBanksResponseBody() {
        }
        
        public GetNFPBanksResponseBody(Blend.ProvidusImplementation.NipWrapperService.ArrayOfString GetNFPBanksResult) {
            this.GetNFPBanksResult = GetNFPBanksResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class fundtransfersingleitem_dcRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="fundtransfersingleitem_dc", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequestBody Body;
        
        public fundtransfersingleitem_dcRequest() {
        }
        
        public fundtransfersingleitem_dcRequest(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class fundtransfersingleitem_dcRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string SessionId;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public string DestinationBankCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=2)]
        public string ChannelCode;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=3)]
        public string DestinationAccountNumber;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=4)]
        public string AccountName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=5)]
        public string OriginatorName;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=6)]
        public string Narration;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=7)]
        public string PaymentReference;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=8)]
        public string Amount;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=9)]
        public string CustomerSourceAccountNumber;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=10)]
        public string BeneficiaryBVN;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=11)]
        public string BeneficiaryBankName;
        
        public fundtransfersingleitem_dcRequestBody() {
        }
        
        public fundtransfersingleitem_dcRequestBody(string SessionId, string DestinationBankCode, string ChannelCode, string DestinationAccountNumber, string AccountName, string OriginatorName, string Narration, string PaymentReference, string Amount, string CustomerSourceAccountNumber, string BeneficiaryBVN, string BeneficiaryBankName) {
            this.SessionId = SessionId;
            this.DestinationBankCode = DestinationBankCode;
            this.ChannelCode = ChannelCode;
            this.DestinationAccountNumber = DestinationAccountNumber;
            this.AccountName = AccountName;
            this.OriginatorName = OriginatorName;
            this.Narration = Narration;
            this.PaymentReference = PaymentReference;
            this.Amount = Amount;
            this.CustomerSourceAccountNumber = CustomerSourceAccountNumber;
            this.BeneficiaryBVN = BeneficiaryBVN;
            this.BeneficiaryBankName = BeneficiaryBankName;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class fundtransfersingleitem_dcResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="fundtransfersingleitem_dcResponse", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponseBody Body;
        
        public fundtransfersingleitem_dcResponse() {
        }
        
        public fundtransfersingleitem_dcResponse(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class fundtransfersingleitem_dcResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string fundtransfersingleitem_dcResult;
        
        public fundtransfersingleitem_dcResponseBody() {
        }
        
        public fundtransfersingleitem_dcResponseBody(string fundtransfersingleitem_dcResult) {
            this.fundtransfersingleitem_dcResult = fundtransfersingleitem_dcResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetNIBSSChargesRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetNIBSSCharges", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequestBody Body;
        
        public GetNIBSSChargesRequest() {
        }
        
        public GetNIBSSChargesRequest(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetNIBSSChargesRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string Amount;
        
        public GetNIBSSChargesRequestBody() {
        }
        
        public GetNIBSSChargesRequestBody(string Amount) {
            this.Amount = Amount;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetNIBSSChargesResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="GetNIBSSChargesResponse", Namespace="http://tempuri.org/", Order=0)]
        public Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponseBody Body;
        
        public GetNIBSSChargesResponse() {
        }
        
        public GetNIBSSChargesResponse(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class GetNIBSSChargesResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string GetNIBSSChargesResult;
        
        public GetNIBSSChargesResponseBody() {
        }
        
        public GetNIBSSChargesResponseBody(string GetNIBSSChargesResult) {
            this.GetNIBSSChargesResult = GetNIBSSChargesResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface NipWrapperServiceSoapChannel : Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NipWrapperServiceSoapClient : System.ServiceModel.ClientBase<Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap>, Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap {
        
        public NipWrapperServiceSoapClient() {
        }
        
        public NipWrapperServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public NipWrapperServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NipWrapperServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NipWrapperServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.nameenquirysingleitem(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest request) {
            return base.Channel.nameenquirysingleitem(request);
        }
        
        public string nameenquirysingleitem(string DestinationBankCode, string AccountNo, string ChannelCode, string SourceAccountNumber) {
            Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequestBody();
            inValue.Body.DestinationBankCode = DestinationBankCode;
            inValue.Body.AccountNo = AccountNo;
            inValue.Body.ChannelCode = ChannelCode;
            inValue.Body.SourceAccountNumber = SourceAccountNumber;
            Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse retVal = ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).nameenquirysingleitem(inValue);
            return retVal.Body.nameenquirysingleitemResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse> Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.nameenquirysingleitemAsync(Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest request) {
            return base.Channel.nameenquirysingleitemAsync(request);
        }
        
        public System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemResponse> nameenquirysingleitemAsync(string DestinationBankCode, string AccountNo, string ChannelCode, string SourceAccountNumber) {
            Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.nameenquirysingleitemRequestBody();
            inValue.Body.DestinationBankCode = DestinationBankCode;
            inValue.Body.AccountNo = AccountNo;
            inValue.Body.ChannelCode = ChannelCode;
            inValue.Body.SourceAccountNumber = SourceAccountNumber;
            return ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).nameenquirysingleitemAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.GetNFPBanks(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest request) {
            return base.Channel.GetNFPBanks(request);
        }
        
        public Blend.ProvidusImplementation.NipWrapperService.ArrayOfString GetNFPBanks() {
            Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequestBody();
            Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse retVal = ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).GetNFPBanks(inValue);
            return retVal.Body.GetNFPBanksResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse> Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.GetNFPBanksAsync(Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest request) {
            return base.Channel.GetNFPBanksAsync(request);
        }
        
        public System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksResponse> GetNFPBanksAsync() {
            Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.GetNFPBanksRequestBody();
            return ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).GetNFPBanksAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.fundtransfersingleitem_dc(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest request) {
            return base.Channel.fundtransfersingleitem_dc(request);
        }
        
        public string fundtransfersingleitem_dc(string SessionId, string DestinationBankCode, string ChannelCode, string DestinationAccountNumber, string AccountName, string OriginatorName, string Narration, string PaymentReference, string Amount, string CustomerSourceAccountNumber, string BeneficiaryBVN, string BeneficiaryBankName) {
            Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequestBody();
            inValue.Body.SessionId = SessionId;
            inValue.Body.DestinationBankCode = DestinationBankCode;
            inValue.Body.ChannelCode = ChannelCode;
            inValue.Body.DestinationAccountNumber = DestinationAccountNumber;
            inValue.Body.AccountName = AccountName;
            inValue.Body.OriginatorName = OriginatorName;
            inValue.Body.Narration = Narration;
            inValue.Body.PaymentReference = PaymentReference;
            inValue.Body.Amount = Amount;
            inValue.Body.CustomerSourceAccountNumber = CustomerSourceAccountNumber;
            inValue.Body.BeneficiaryBVN = BeneficiaryBVN;
            inValue.Body.BeneficiaryBankName = BeneficiaryBankName;
            Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse retVal = ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).fundtransfersingleitem_dc(inValue);
            return retVal.Body.fundtransfersingleitem_dcResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse> Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.fundtransfersingleitem_dcAsync(Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest request) {
            return base.Channel.fundtransfersingleitem_dcAsync(request);
        }
        
        public System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcResponse> fundtransfersingleitem_dcAsync(string SessionId, string DestinationBankCode, string ChannelCode, string DestinationAccountNumber, string AccountName, string OriginatorName, string Narration, string PaymentReference, string Amount, string CustomerSourceAccountNumber, string BeneficiaryBVN, string BeneficiaryBankName) {
            Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.fundtransfersingleitem_dcRequestBody();
            inValue.Body.SessionId = SessionId;
            inValue.Body.DestinationBankCode = DestinationBankCode;
            inValue.Body.ChannelCode = ChannelCode;
            inValue.Body.DestinationAccountNumber = DestinationAccountNumber;
            inValue.Body.AccountName = AccountName;
            inValue.Body.OriginatorName = OriginatorName;
            inValue.Body.Narration = Narration;
            inValue.Body.PaymentReference = PaymentReference;
            inValue.Body.Amount = Amount;
            inValue.Body.CustomerSourceAccountNumber = CustomerSourceAccountNumber;
            inValue.Body.BeneficiaryBVN = BeneficiaryBVN;
            inValue.Body.BeneficiaryBankName = BeneficiaryBankName;
            return ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).fundtransfersingleitem_dcAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.GetNIBSSCharges(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest request) {
            return base.Channel.GetNIBSSCharges(request);
        }
        
        public string GetNIBSSCharges(string Amount) {
            Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequestBody();
            inValue.Body.Amount = Amount;
            Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse retVal = ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).GetNIBSSCharges(inValue);
            return retVal.Body.GetNIBSSChargesResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse> Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap.GetNIBSSChargesAsync(Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest request) {
            return base.Channel.GetNIBSSChargesAsync(request);
        }
        
        public System.Threading.Tasks.Task<Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesResponse> GetNIBSSChargesAsync(string Amount) {
            Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest inValue = new Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequest();
            inValue.Body = new Blend.ProvidusImplementation.NipWrapperService.GetNIBSSChargesRequestBody();
            inValue.Body.Amount = Amount;
            return ((Blend.ProvidusImplementation.NipWrapperService.NipWrapperServiceSoap)(this)).GetNIBSSChargesAsync(inValue);
        }
    }
}
