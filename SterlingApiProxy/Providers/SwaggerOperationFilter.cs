using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Swashbuckle.Swagger;
using System.Web.Http.Description;

namespace SterlingApiProxy.Providers
{
    public class SwaggerOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters != null)
            {
                operation.operationId += "By";
                foreach (var parm in operation.parameters)
                {
                    operation.operationId += string.Format("{0}",
                        parm.name);
                }
            }
        }
    }
}