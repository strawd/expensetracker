// Copyright 2016 David Straw

using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json.Linq;

namespace ExpenseTracker.Controllers
{
    [MobileAppController]
    public class SimpleController : ApiController
    {
        public object Get()
        {
            var result = new JObject();
            result.Add("requestContext_route", JToken.FromObject(RequestContext.RouteData.Route));
            result.Add("requestContext_routeValues", JToken.FromObject(RequestContext.RouteData.Values));
            result.Add("actionDescriptor_properties", JToken.FromObject(ActionContext.ActionDescriptor.Properties));
            result.Add("actionArguments", JToken.FromObject(ActionContext.ActionArguments));
            result.Add("actionSupportedMethods", JToken.FromObject(ActionContext.ActionDescriptor.SupportedHttpMethods));
            result.Add("requestMethod", JToken.FromObject(Request.Method));
            result.Add("requestUri", JToken.FromObject(Request.RequestUri));
            result.Add("requestHeaders", JToken.FromObject(Request.Headers));
            return result;
        }

        public object Post()
        {
            var result = new JObject();
            result.Add("requestContext_route", JToken.FromObject(RequestContext.RouteData.Route));
            result.Add("requestContext_routeValues", JToken.FromObject(RequestContext.RouteData.Values));
            result.Add("actionDescriptor_properties", JToken.FromObject(ActionContext.ActionDescriptor.Properties));
            result.Add("actionArguments", JToken.FromObject(ActionContext.ActionArguments));
            result.Add("actionSupportedMethods", JToken.FromObject(ActionContext.ActionDescriptor.SupportedHttpMethods));
            result.Add("requestMethod", JToken.FromObject(Request.Method));
            result.Add("requestUri", JToken.FromObject(Request.RequestUri));
            result.Add("requestHeaders", JToken.FromObject(Request.Headers));
            return result;
        }
    }
}