using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Services.Description;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using AppZoneMiddleware.API.Infrastructure;

[assembly: OwinStartup(typeof(AppZoneMiddleware.API.Startup))]

namespace AppZoneMiddleware.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            Configuration(app);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore();

            services.AddSwaggerGen(c =>
            {
                c.TagActionsBy(api => api.HttpMethod);
                //c.DocumentFilter<SwaggerOperationNameResolver>();

                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
            // Add framework services.
        }
    }
}
