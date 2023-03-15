using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using AppZoneMiddleware.Shared.Utility;
using Blend.DefaultImplementation.Persistence;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Blend.DefaultImplementation.Utility
{
    public class ServiceDescriptionAssembler
    {
        public static Dictionary<string, Assembly> _assemblies;
        public string EntityAPIEndpoint = System.Configuration.ConfigurationManager.AppSettings.Get("EntityAPIEndpoint");

        public static void Initialize()
        {
            _assemblies = new Dictionary<string, Assembly>();
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            List<ApiSecuritySpec> securitySpecs = repository.Get().Where(x => x.isSoap).ToList();
            foreach (var item in securitySpecs)
            {
                try
                {
                    System.Web.Services.Description.ServiceDescription description = null;
                    using (TextReader sr = new StringReader(item.ServiceDescription))
                    {
                        description = System.Web.Services.Description.ServiceDescription.Read(sr);
                    }
                    var sec = MetadataSection.CreateFromServiceDescription(description);
                    var metaDocs = new MetadataSet(new List<MetadataSection> { sec });

                    WsdlImporter importer = new WsdlImporter(metaDocs);
                    ServiceContractGenerator generator = new ServiceContractGenerator()
                    {
                        Options = ServiceContractGenerationOptions.ClientClass | ServiceContractGenerationOptions.TypedMessages
                    };
                    Collection<ContractDescription> contracts = importer.ImportAllContracts();
                    importer.ImportAllEndpoints();

                    foreach (ContractDescription contract in contracts)
                    {
                        generator.GenerateServiceContractType(contract);
                    }
                    if (generator.Errors.Count != 0)
                    {
                        Logger.LogError(new Exception("There were errors during code compilation."));
                        continue;
                    }

                    ////Generate the proxy code
                    CodeDomProvider provider1 = CodeDomProvider.CreateProvider("CSharp");

                    // Compile the assembly proxy with the appropriate references
                    var assemblyReferences = new CompilerParameters(new[] { "System.dll", "System.Web.Services.dll", "System.Web.dll", "System.Xml.dll", "System.Data.dll", "System.Runtime.Serialization.dll" });
                    assemblyReferences.GenerateExecutable = false;

                    //CompilerParameters parms = new CompilerParameters(assemblyReferences);
                    assemblyReferences.ReferencedAssemblies.Add(typeof(EndpointAddress).Assembly.Location);

                    CompilerResults results = provider1.CompileAssemblyFromDom(assemblyReferences, generator.TargetCompileUnit);

                    //Check For Errors
                    if (results.Errors.Count > 0)
                    {
                        foreach (CompilerError oops in results.Errors)
                        {
                            Logger.LogError(new Exception("========Compiler error============"));
                            Logger.LogError(new Exception(oops.ErrorText));
                        }
                        throw new Exception("Compile Error Occured calling webservice. Check log file.");
                    }
                    Assembly serviceAssembly = results.CompiledAssembly;

                    _assemblies.Add(item.URL, serviceAssembly);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.InnerException == null ? ex : ex.InnerException);
                    continue;
                }

            }
        }
    }
}
