using AppZoneMiddleware.Shared.Entities;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace AppZoneMiddleware.API.Infrastructure
{
    /// <summary>
    /// To configure services that are accessible
    /// </summary>
    public class AllowedServices
    {
        private static Dictionary<string, List<string>> _allowedServices = new Dictionary<string, List<string>>();
        private static string FilePath = ConfigurationManager.AppSettings.Get("ServicesFilePath");
        private static string FileName = ConfigurationManager.AppSettings.Get("FileName");
        private static string ZipPath = ConfigurationManager.AppSettings.Get("ServiceZipPath");

        public static void Initialize()
        {
            var dependencyManagers = AppDomain.CurrentDomain.GetAssemblies()
               .SelectMany(a => a.ExportedTypes)
               .Where(t => typeof(DbContext).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            //using (ZipFile zipfile = new ZipFile(ZipPath))
            //{
            //    ZipEntry theEntry = zipfile.Entries.Where(e => e.FileName == "services.txt").FirstOrDefault();
            //    if (theEntry != null)
            //    {
            //        theEntry.Password = "jide2010";
            //        theEntry.Extract(@"C: \Users\RONNIE\Desktop\");
            //    }
            //}
            string jsonString = File.ReadAllText(FilePath + FileName).ToLower();

             _allowedServices = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonString);
            //File.Delete(FilePath + FileName);
        }

        public static bool HasServiceAccess(string solution, string service)
        {
            if (!_allowedServices.ContainsKey(solution.ToLower()))
                return false;

            return _allowedServices[solution.ToLower()].Contains(service.ToLower().Replace("controller", ""));
        }

        public Dictionary<string, List<string>> AddServiceToList(UpdateServiceRequest request, out string ErrorMessage)
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
                    return _allowedServices;
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
        public Dictionary<string, List<string>> DisplayServices(string password)
        {
            return _allowedServices;
            //Dictionary<string, List<string>> _allowedServices = null;
            //using (ZipFile zipfile = new ZipFile(@"C:\Users\RONNIE\Desktop\services.zip"))
            //{
            //    zipfile.Password = "jide2010";
            //    ZipEntry theEntry = zipfile.Entries.Where(e => e.FileName == "services.txt").FirstOrDefault();
            //    if (theEntry != null)
            //    {
            //        theEntry.Password = password;
            //        theEntry.Extract(@"C: \Users\RONNIE\Desktop\");
            //    }
            //}

            //string jsonString = File.ReadAllText(@"C:\Users\RONNIE\Desktop\services.txt");
            //_allowedServices = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonString);

            //return _allowedServices;
        }
    }
}