using AppZoneMiddleware.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppZoneMiddleware.Shared.Entities.AuthenticationProxy;
using Blend.DefaultImplementation.Persistence;
using AppZoneMiddleware.Shared.Utility;

namespace Blend.DefaultImplementation
{
    public class APISecurityService : IAPISecurityService
    {
        /// <summary>
        /// This methods uses the request message to add a new api security specification to the list of apis already provided
        /// </summary>
        /// <param name="AddRequest">The api security specification to be added</param>
        /// <returns></returns>
        public AddAPISecResponse AddAPI(ApiSecuritySpec AddRequest)
        {
            Logger.LogInfo("APISecurityService.AddAPI.Input", AddRequest);
            ApplicationDbContext context = new ApplicationDbContext();
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            ApiSecuritySpec securitySpec = context.APISecuritySpecs.SqlQuery(string.Format("select * from APISecuritySpecs where URL like '%{0}%' ", AddRequest.URL)).ToList().FirstOrDefault();

            if (securitySpec == null)
            {
                repository.Save(AddRequest);
                securitySpec = context.APISecuritySpecs.SqlQuery(string.Format("select * from APISecuritySpecs where URL like '%{0}%' ", AddRequest.URL)).ToList().FirstOrDefault();
                return new AddAPISecResponse { ResponseCode = "00", ResponseDescription = "API successfully added", ApiSecurity = securitySpec };
            }
            else
            {
                return new AddAPISecResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Cannot specified already exists"
                };
            }
        }

        public List<ApiSecuritySpec> GetAPIs()
        {
            Logger.LogInfo("APISecurityService.GetAPIs","");
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            List<ApiSecuritySpec> securitySpec = repository.Get().ToList();
            return securitySpec;
        }

        public AddAPISecResponse UpdateAPI(ApiSecuritySpec UpdateRequest)
        {
            Logger.LogInfo("APISecurityService.UpdateAPI.Input", UpdateRequest);
            ApplicationDbContext context = new ApplicationDbContext();
            ContextRepository<ApiSecuritySpec> repository = new ContextRepository<ApiSecuritySpec>();
            ApiSecuritySpec securitySpec = context.APISecuritySpecs.SqlQuery(string.Format("select * from APISecuritySpecs where URL like '%{0}%' ", UpdateRequest.URL)).ToList().FirstOrDefault();

            if (securitySpec != null)
            {
                repository.Update(UpdateRequest);
                securitySpec = context.APISecuritySpecs.SqlQuery(string.Format("select * from APISecuritySpecs where URL like '%{0}%' ", UpdateRequest.URL)).ToList().FirstOrDefault();
                return new AddAPISecResponse { ResponseCode = "00", ResponseDescription = "API successfully updated", ApiSecurity = securitySpec };
            }
            else
            {
                return new AddAPISecResponse
                {
                    ResponseCode = "00",
                    ResponseDescription = "Cannot specified does not exists"
                };
            }
        }
    }
}
