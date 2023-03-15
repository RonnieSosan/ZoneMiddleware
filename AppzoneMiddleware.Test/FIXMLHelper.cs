using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppzoneMiddleware.Test
{
    public class FIXMLHelper
    {
        public FIXML InitializeFI(Dictionary<string, object> input)
        {
            FIXML fiXml = new FIXML()
            {
                Header = new FIXMLHeader
                {
                    RequestHeader = new FIXMLHeaderRequestHeader
                    {
                        MessageKey = new FIXMLHeaderRequestHeaderMessageKey
                        {
                            RequestUUID = "Req_1426162415487",
                            ChannelId = "CRM",
                            LanguageId = input.ContainsKey("LanguageId") ? input["LanguageId"].ToString() : "",
                            ServiceRequestId = "RetCustAdd",
                            ServiceRequestVersion = 10.2M,
                        },
                        RequestMessageInfo = new FIXMLHeaderRequestHeaderRequestMessageInfo
                        {
                            ArmCorrelationId = input.ContainsKey("ArmCorrelationId") ? input["ArmCorrelationId"].ToString() : "",
                            BankId = input.ContainsKey("BankId") ? input["BankId"].ToString() : "",
                            EntityId = "",
                            EntityType = "",
                            TimeZone = "",
                            MessageDateTime = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff")
                        },
                        Security = new FIXMLHeaderRequestHeaderSecurity
                        {
                            FICertToken = "",
                            RealUser = "",
                            RealUserLoginSessionId = "",
                            RealUserPwd = "",
                            SSOTransferToken = "",
                            Token = new FIXMLHeaderRequestHeaderSecurityToken
                            {
                                PasswordToken = new FIXMLHeaderRequestHeaderSecurityTokenPasswordToken
                                {
                                    Password = "",
                                    UserId = ""
                                }
                            }
                        }
                    }
                },
                Body = new FIXMLBody
                {
                    RetCustAddRequest = new FIXMLBodyRetCustAddRequest
                    {
                        RetCustAddRq = new FIXMLBodyRetCustAddRequestRetCustAddRq
                        {
                            CustDtls = new FIXMLBodyRetCustAddRequestRetCustAddRqCustDtls
                            {
                                CustData = new FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustData
                                {
                                    AddrDtls = new FIXMLBodyRetCustAddRequestRetCustAddRqCustDtlsCustDataAddrDtls
                                    {
                                        AddrLine1 = input.ContainsKey("AddrLine1") ? input["AddrLine1"].ToString() : "",
                                        AddrLine2 = input.ContainsKey("AddrLine2") ? input["AddrLine2"].ToString() : "",
                                        AddrLine3 = input.ContainsKey("AddrLine3") ? input["AddrLine3"].ToString() : "",
                                        City = input.ContainsKey("City") ? input["City"].ToString() : "",
                                        State = input.ContainsKey("State") ? input["State"].ToString() : "",
                                        Country = input.ContainsKey("Country") ? input["Country"].ToString() : "",
                                        AddrCategory = input.ContainsKey("AddrCategory") ? input["AddrCategory"].ToString() : "",
                                        HoldMailFlag = input.ContainsKey("HoldMailFlag") ? input["HoldMailFlag"].ToString() : "",
                                        HouseNum = input.ContainsKey("HouseNum") ? input["HouseNum"].ToString() : "",
                                        PrefAddr = input.ContainsKey("PrefAddr") ? input["PrefAddr"].ToString() : "",
                                        PrefFormat = input.ContainsKey("PrefFormat") ? input["PrefFormat"].ToString() : "",
                                        StreetName = input.ContainsKey("StreetName") ? input["StreetName"].ToString() : "",
                                        StreetNum = input.ContainsKey("StreetNum") ? input["StreetNum"].ToString() : "",
                                        StartDt = input.ContainsKey("StartDt") ? input["StartDt"].ToString() : "",
                                        PostalCode = input.ContainsKey("PostalCode") ? input["PostalCode"].ToString() : "",
                                    }
                                }
                            },
                            RelatedDtls = new FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtls
                            {
                                DemographicData = new FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsDemographicData
                                {
                                    MaritalStatus = input.ContainsKey("BankId") ? input["MaritalStatus"].ToString() : "",
                                    Nationality = input.ContainsKey("BankId") ? input["Nationality"].ToString() : "",
                                },
                                EntityDoctData = new FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsEntityDoctData
                                {
                                    CountryOfIssue = input.ContainsKey("CountryOfIssue") ? input["CountryOfIssue"].ToString() : "",
                                    DocCode = input.ContainsKey("DocCode") ? input["DocCode"].ToString() : "",
                                    IssueDt = input.ContainsKey("IssueDt") ? input["IssueDt"].ToString() : "",
                                    ExpDt = input.ContainsKey("ExpDt") ? input["ExpDt"].ToString() : "",
                                    PlaceOfIssue = input.ContainsKey("PlaceOfIssue") ? input["PlaceOfIssue"].ToString() : "",
                                    ReferenceNum = input.ContainsKey("ReferenceNum") ? input["ReferenceNum"].ToString() : "",
                                    TypeCode = input.ContainsKey("TypeCode") ? input["TypeCode"].ToString() : "",
                                },
                                PsychographicData = new FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicData
                                {
                                    CustCurrCode = input.ContainsKey("CustCurrCode") ? input["CustCurrCode"].ToString() : "",
                                    PsychographMiscData = new FIXMLBodyRetCustAddRequestRetCustAddRqRelatedDtlsPsychographicDataPsychographMiscData
                                    {
                                        StrText10 = input.ContainsKey("StrText10") ? input["StrText10"].ToString() : "",
                                        Type = input.ContainsKey("Type") ? input["Type"].ToString() : "",
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return fiXml;
        }

        public XElement GenerateFiXMlHeader(Dictionary<string, object> source)
        {
            XElement Header = new XElement("Header",
                new XElement("RequestHeader",
                    new XElement("MessageKey",
                        new XElement("RequestUUID", "Req_1426162415487"),
                        new XElement("ServiceRequestId", "RetCustAdd"),
                        new XElement("ServiceRequestVersion", "10.2"),
                        new XElement("ChannelId", "CRM"),
                        new XElement("LanguageId", "")),
                    new XElement("RequestMessageInfo",
                        new XElement("BankId", ""),
                        new XElement("TimeZone", ""),
                        new XElement("EntityId", ""),
                        new XElement("EntityType", ""),
                        new XElement("ArmCorrelationId", ""),
                        new XElement("MessageDateTime", DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss.fff"))),
                    new XElement("Security",
                        new XElement("Token",
                            new XElement("PasswordToken",
                                new XElement("UserId", ""),
                                new XElement("Password", ""))),
                        new XElement("FICertToken", ""),
                        new XElement("RealUserLoginSessionId", ""),
                        new XElement("RealUser", ""),
                        new XElement("RealUserPwd", ""),
                        new XElement("SSOTransferToken", "")
                        )));
            return Header;
        }

        public XElement GenerateFIXMLBody(Dictionary<string, object> source)
        {
            XElement Body = new XElement("Body",
                new XElement("RetCustAddRequest",
                    new XElement("RetCustAddRq",
                        new XElement("CustDtls",
                            new XElement("CustData",
                                new XElement("AddrDtls",
                                    new XElement("AddrLine1", source.ContainsKey("AddrLine1") ? source["AddrLine1"].ToString() : "100"),
                                    new XElement("AddrLine2", source.ContainsKey("AddrLine2") ? source["AddrLine2"].ToString() : ""),
                                    new XElement("AddrLine3", source.ContainsKey("AddrLine3") ? source["AddrLine3"].ToString() : ""),
                                    new XElement("AddrCategory", source.ContainsKey("AddrCategory") ? source["AddrCategory"].ToString() : ""),
                                    new XElement("City", source.ContainsKey("City") ? source["City"].ToString() : ""),
                                    new XElement("Country", source.ContainsKey("Country") ? source["Country"].ToString() : ""),
                                    new XElement("HoldMailFlag", source.ContainsKey("HoldMailFlag") ? source["HoldMailFlag"].ToString() : ""),
                                    new XElement("HouseNum", source.ContainsKey("HouseNum") ? source["HouseNum"].ToString() : ""),
                                    new XElement("PrefAddr", source.ContainsKey("PrefAddr") ? source["PrefAddr"].ToString() : ""),
                                    new XElement("PrefFormat", source.ContainsKey("PrefFormat") ? source["PrefFormat"].ToString() : ""),
                                    new XElement("StartDt", source.ContainsKey("StartDt") ? source["StartDt"].ToString() : ""),
                                    new XElement("State", source.ContainsKey("State") ? source["State"].ToString() : ""),
                                    new XElement("StreetName", source.ContainsKey("StreetNum") ? source["StreetNum"].ToString() : ""),
                                    new XElement("StreetNum", source.ContainsKey("StreetNum") ? source["StreetNum"].ToString() : ""),
                                    new XElement("PostalCode", source.ContainsKey("PostalCode") ? source["PostalCode"].ToString() : "")),
                                new XElement("BirthDt", ""),
                                new XElement("BirthMonth", ""),
                                new XElement("BirthYear", ""),
                                new XElement("CreatedBySystemId", ""),
                                new XElement("DateOfBirth", ""),
                                new XElement("CustHealthCode", ""),
                                new XElement("Language", "UK (English)"),
                                new XElement("LastName", "clement"),
                                new XElement("MiddleName", ""),
                                new XElement("IsMinor", ""),
                                new XElement("IsCustNRE", ""),
                                new XElement("DefaultAddrType", ""),
                                new XElement("Gender", ""),
                                new XElement("Manager", ""),
                                new XElement("NativeLanguageCode", ""),
                                new XElement("Occupation", ""),
                                new XElement("OperationsReasonCode",
                                    new XElement("ExpDt", "")),
                                new XElement("PhoneEmailDtls",
                                    new XElement("Email", "clement.ojo@live.com"),
                                    new XElement("EndDt", ""),
                                    new XElement("PhoneEmailType", ""),
                                    new XElement("PhoneNum", ""),
                                    new XElement("PhoneNumCityCode", ""),
                                    new XElement("PhoneNumCountryCode", ""),
                                    new XElement("PhoneNumLocalCode", ""),
                                    new XElement("PhoneOrEmail", ""),
                                    new XElement("PrefFlag", "Y"),
                                    new XElement("StartDt", "")),
                                new XElement("PrefName", "CLEMENT"),
                                new XElement("PrefPhoneType", "CELLPH"),
                                new XElement("PrimarySolId", "001"),
                                new XElement("PassportExpDt", "2018-01-01T00:00:00.000"),
                                new XElement("PassportIssueDt", ""),
                                new XElement("RelationshipOpeningDt", ""),
                                new XElement("Salutation", ""),
                                new XElement("SecondaryRMId", ""),
                                new XElement("SegmentationClass", ""),
                                new XElement("StaffFlag", ""),
                                new XElement("StartDt", ""),
                                new XElement("SubSegment", "OTHER"),
                                new XElement("TaxDeductionTable", "C5.00"))),
                        new XElement("RelatedDtls",
                                new XElement("DemographicData",
                                    new XElement("MaritalStatus", ""),
                                    new XElement("Nationality", "")),
                                new XElement("EntityDoctData",
                                    new XElement("CountryOfIssue", "NG"),
                                    new XElement("DocCode", "PSPRT"),
                                    new XElement("ExpDt", ""),
                                    new XElement("IssueDt", ""),
                                    new XElement("TypeCode", ""),
                                    new XElement("PlaceOfIssue", ""),
                                    new XElement("ReferenceNum", "")),
                                new XElement("PsychographicData",
                                    new XElement("CustCurrCode", "NGN"),
                                    new XElement("PsychographMiscData",
                                        new XElement("StrText10", ""),
                                        new XElement("Type", ""))
                                    ))
                    )));

            return Body;
        }
    }
}
