using AppZoneMiddleware.Shared.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Blend.GTBImplementation
{
    public class BillPaymentService
    {
        public string EntityAPIEndpoint = System.Configuration.ConfigurationManager.AppSettings.Get("EntityAPIEndpoint");
        public string InstitutionID = System.Configuration.ConfigurationManager.AppSettings.Get("InstitutionID");
        public string MainServiceID = System.Configuration.ConfigurationManager.AppSettings.Get("MainServiceID");

        public List<JObject> GetQuicktellerBillersByCategory(string CategroyId)
        {
            Logger.LogInfo("BillPaymentService.GetQuicktellerBillersByCategory.Input: ", CategroyId);
            List<JObject> arrayOfBillerForms = new List<JObject>();
            var genericPageToSave = new JObject();
            int counter = 0;
            using (HttpClient client = new HttpClient())
            {
                var requestBillers = new { categoryId = CategroyId, requestId = "123", channel = "GTHUB", userId = "GTHUB", sessionId = "1234" };
                HttpResponseMessage httpResponse = client.PostAsync("http://gtweb.gtbank.com/WEBAPIs/Pub/GTBBillPaymentService/Api/GetFormsByCategoryId", new StringContent(JsonConvert.SerializeObject(requestBillers), Encoding.UTF8, "application/json")).Result;
                string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                JObject responseFromApi = JsonConvert.DeserializeObject<JObject>(rawResponse);
                arrayOfBillerForms = JsonConvert.DeserializeObject<List<JObject>>(Convert.ToString(responseFromApi["formDetails"]));
                foreach (var billerForm in arrayOfBillerForms)
                {

                    billerForm.Add("formFields", GetFormDetails(Convert.ToString(billerForm["formId"])));
                    var genericPage = ConvertFormToGenericPage(billerForm);
                    billerForm.Add("GenericPage", genericPage);
                    if (counter == 0)
                        genericPageToSave.Add("Entity", genericPage);
                    counter++;
                }
            }


            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage httpRequest = new HttpRequestMessage() { RequestUri = new Uri(string.Format("{0}/api/Entities/{1}/0_Generic page", EntityAPIEndpoint, InstitutionID)), Method = HttpMethod.Post, Content = new StringContent(JsonConvert.SerializeObject(genericPageToSave), Encoding.UTF8, "application/json") };
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
                }
            }
            return arrayOfBillerForms;
        }

        public JArray GetFormDetails(string fromID)
        {
            Logger.LogInfo("BillPaymentService.GetFormDetails.Input: ", fromID);
            JArray arrayOfFormFields = new JArray();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var requestBillers = new { formId = fromID, requestId = "123", channel = "GTHUB", userId = "GTHUB", sessionId = "1234" };
                    HttpResponseMessage httpResponse = client.PostAsync("http://gtweb.gtbank.com/WEBAPIs/Pub/GTBBillPaymentService/Api/GetFormFieldsByFormId", new StringContent(JsonConvert.SerializeObject(requestBillers), Encoding.UTF8, "application/json")).Result;
                    string rawResponse = httpResponse.Content.ReadAsStringAsync().Result;
                    JObject responseFromApi = JsonConvert.DeserializeObject<JObject>(rawResponse);
                    arrayOfFormFields = JsonConvert.DeserializeObject<JArray>(Convert.ToString(responseFromApi["formFields"]));

                }
            }
            catch (Exception ex)
            {

            }
            return arrayOfFormFields;
        }

        public JObject ConvertFormToGenericPage(JObject BillerForm)
        {
            Logger.LogInfo("BillPaymentService.ConvertFormToGenericPage.Input: ", BillerForm);
            JObject genericPage = new JObject();
            var formDetails = JsonConvert.DeserializeObject<List<JObject>>(Convert.ToString(BillerForm["formFields"]));
            if (formDetails == null)
            {
                return null;
            }
            var genericFields = new JArray();
            genericPage.Add("Name", Convert.ToString(BillerForm["customerName"]));
            genericPage.Add("Type", "Page");
            genericPage.Add("Is New", "True");
            genericPage.Add("Page Category", "1");
            genericPage.Add("Main Service ID", MainServiceID);

            foreach (var field in formDetails)
            {
                JObject fieldValue = new JObject();
                foreach (var property in field)
                {
                    if (property.Key == "field_name")
                        fieldValue.Add("Name", Convert.ToString(property.Value));
                    if (property.Key == "field_caption")
                        fieldValue.Add("FieldLabel", Convert.ToString(property.Value));
                    if (property.Key == "field_type")
                        fieldValue.Add("Type", "Text");
                    if (property.Key == "tooltip")
                        fieldValue.Add("Hint", Convert.ToString(property.Value));
                    if (property.Key == "read_only" && Convert.ToBoolean(Convert.ToInt32(property.Value)))
                        fieldValue["Type"] = "ReadOnly";
                    if (property.Key == "field_mandatory")
                        fieldValue.Add("Parameter", new JObject { { "Validation", new JObject { { "IsRequired", "True" } } } });
                    if (property.Key == "field_lov" && Convert.ToString(property.Value) != "0")
                    {
                        fieldValue["Type"] = "DataList";
                        fieldValue.Add("Entity", "19");
                    }
                    else
                    {
                        fieldValue.Add(property.Key, Convert.ToString(property.Value));
                    }
                }
                genericFields.Add(fieldValue);
            }
            var section = JsonConvert.SerializeObject(new JArray { new JObject { { "Name", "" }, { "NumberofColumn", "1" }, { "Buttons", new JArray() }, { "Fields", genericFields } } });
            genericPage.Add("Description", Convert.ToString(BillerForm["formDescription"]));
            genericPage.Add("Sections", section);
            return genericPage;
        }
    }
}
