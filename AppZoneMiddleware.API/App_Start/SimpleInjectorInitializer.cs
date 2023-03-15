[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(AppZoneMiddleware.API.App_Start.SimpleInjectorInitializer), "Initialize")]

namespace AppZoneMiddleware.API.App_Start
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using SimpleInjector;
    using SimpleInjector.Integration.WebApi;
    using System.Web.Http;
    using System.Reflection;
    using Shared;

    public class SimpleInjectorInitializer
    {
        /// <summary>Initialize the container and register it as Web API Dependency Resolver.</summary>
        /// 
        public static void Initialize()
        {

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new SimpleInjector.Lifestyles.AsyncScopedLifestyle();    // WebApiRequestLifestyle has been deprecated, so it was replaced.

            InitializeContainer(container);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static void InitializeContainer(Container container)
        {
            var dependencyManagers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => typeof(IServiceCollectionExtension).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                .Select(t => Activator.CreateInstance(t) as IServiceCollectionExtension);

            foreach (var dependencyManager in dependencyManagers)
            {
                dependencyManager.ServiceInitializer(container);
            }
        }
    }
}