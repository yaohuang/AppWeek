using LunchBuddies.Models;
using Microsoft.Data.Edm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Newtonsoft.Json.Serialization;

namespace LunchBuddies
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapODataRoute("odata", "odata", GetEdmModel());
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );


            // Use camel case for JSON data.
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }

        private static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<User>("Users");
            builder.EntitySet<LunchRequest>("LunchRequests");
            builder.EntitySet<UserLunchRequest>("UserLunchRequests");
            return builder.GetEdmModel();
        }
    }
}
