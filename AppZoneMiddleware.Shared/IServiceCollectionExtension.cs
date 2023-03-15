using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppZoneMiddleware.Shared
{
    public interface IServiceCollectionExtension
    {
        void ServiceInitializer(Container container);
    }
}
