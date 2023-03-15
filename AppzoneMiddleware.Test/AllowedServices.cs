using AppZoneMiddleware.Shared.Entities;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppzoneMiddleware.Test
{
    public class AllowedServices
    {
        private static Dictionary<string, List<string>> _allowedServices = new Dictionary<string, List<string>>();
        private static string FilePath = System.Configuration.ConfigurationManager.AppSettings.Get("ServicesFilePath");
        private static string FileName = System.Configuration.ConfigurationManager.AppSettings.Get("FileName");
        private static string ZipPath = System.Configuration.ConfigurationManager.AppSettings.Get("ServiceZipPath");

        public AllowedServices()
        {
            using (ZipFile zipfile = new ZipFile(ZipPath))
            {
                ZipEntry theEntry = zipfile.Entries.Where(e => e.FileName == "services.txt").FirstOrDefault();
                if (theEntry != null)
                {
                    theEntry.Password = "jide2010";
                    theEntry.Extract(@"C: \Users\RONNIE\Desktop\");
                }
            }

            string jsonString = File.ReadAllText(FilePath + FileName).ToLower();

            _allowedServices = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonString);
            File.Delete(ZipPath);
            File.Delete(FilePath + FileName);
        }
        public Dictionary<string, List<string>> AddeServiceToList(UpdateServiceRequest request, out string ErrorMessage)
        {
            ErrorMessage = "";

            string key = request.Solution.ToLower();
            string value = request.Service.ToLower();

            if (_allowedServices.ContainsKey(key))
            {
                if (!_allowedServices[key].Contains(value))
                    _allowedServices[key].Add(value);
                else
                {
                    ErrorMessage = "service already exists";
                }
            }
            else
                _allowedServices.Add(key, new List<string>() { value });

            string services = Newtonsoft.Json.JsonConvert.SerializeObject(_allowedServices);

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "jide2010";
                zip.AddEntry(FileName, services);
                zip.Save(ZipPath);
            }

            return _allowedServices;
        }

        public Dictionary<string, List<string>> RemoveServiceFromList(UpdateServiceRequest request, out string ErrorMessage)
        {
            ErrorMessage = "";

            string key = request.Solution.ToLower();
            string value = request.Service.ToLower();

            if (_allowedServices.ContainsKey(key))
            {
                if (_allowedServices[key].Contains(value))
                    _allowedServices[key].Remove(value);
                else
                {
                    ErrorMessage = "service does not exists";
                }
            }
            else
                _allowedServices.Add(key, new List<string>() { value });

            string services = Newtonsoft.Json.JsonConvert.SerializeObject(_allowedServices);

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "jide2010";
                zip.AddEntry(FileName, services);
                zip.Save(ZipPath);
            }

            return _allowedServices;
        }
    }
}
