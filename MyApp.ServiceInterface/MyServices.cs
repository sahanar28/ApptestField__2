using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Script;
using ServiceStack.DataAnnotations;
using MyApp.ServiceModel;
using ServiceStack.Configuration;
using static MyApp.ServiceInterface.MyServices;


namespace MyApp.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }

        [Authenticate]
        public object Any(RequiresAuth request)
        {
            return new RequiresAuthResponse { Result = $"Hello, {request.Name}!" };
        }

        [RequiredRole("Manager")]
        public object Any(RequiresRole request)
        {
            return new RequiresRoleResponse { Result = $"Hello, {request.Name}!" };
        }

        [RequiredRole(nameof(RoleNames.Admin))]
        public object Any(RequiresAdmin request)
        {
            return new RequiresAdminResponse { Result = $"Hello, {request.Name}!" };
        }

        public object Get(Hello request)
        {
            var response = new HelloResponse
            {
                Fields = new Dto
                {
                    StringDictionary = new Dictionary<string, string>
            {
                { "Key1", "value1" },
                { "Key2", "value2" }
            }
                }
            };

            return response;
        }

    }
}