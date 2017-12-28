using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Security.OAuth;
using WebApiThrottle;
using TraceLevel = System.Web.Http.Tracing.TraceLevel;

namespace ACTransit.CusRel.Public.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("HttpConfiguration is null.");

            config.Services.Add(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            // Web API configuration and services
#if DEBUG
            var traceWriter = config.EnableSystemDiagnosticsTracing();
            traceWriter.IsVerbose = true;
            traceWriter.MinimumLevel = TraceLevel.Debug;
#endif

            // enable Cross Origin Resource Sharing (CORS) 
            var cors = new EnableCorsAttribute("*", "*", "*")  // TODO: change to your.company.dns/path/to/CusRel Website 
            {
                SupportsCredentials = true
            }; 
            config.EnableCors(cors);

            // Configure Web API to use only bearer token authentication.            
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // add XML and JSON formatters for requests and responses
            config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", XmlMediaTypeFormatter.DefaultMediaType);
            config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", JsonMediaTypeFormatter.DefaultMediaType);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultActionIdApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultActionIdKeyApi",
                routeTemplate: "{controller}/{action}/{id}/{key}"
            );

            config.Routes.MapHttpRoute(
                name: "DefaultIdActionApi",
                routeTemplate: "{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Web API throttling
            var throttlingHandler = new ThrottlingHandler()
            {
                Policy = new ThrottlePolicy(perSecond: 20, perMinute: 300)//, perHour: 36000, perDay: 864000, perWeek: 6048000)
                {
                    //scope to IPs
                    IpThrottling = true,
                    //IpRules = new Dictionary<string, RateLimits>
                    //{
                    //    {"::1/10", new RateLimits {PerSecond = 2}},
                    //    {"192.168.2.1", new RateLimits {PerMinute = 30, PerHour = 30*60, PerDay = 30*60*24}}
                    //},
                    //white list the "::1" IP to disable throttling on localhost for Win8
                    IpWhitelist = new List<string> { "::1", "127.0.0.1", "192.168.0.0/16", "10.0.0.0/8", "172.16.0.0/12" },

                    //scope to clients (if IP throttling is applied then the scope becomes a combination of IP and client key)
                    //ClientThrottling = true,
                    //ClientRules = new Dictionary<string, RateLimits>
                    //{
                    //    {"api-client-key-1", new RateLimits {PerMinute = 60, PerHour = 600}},
                    //    {"api-client-key-9", new RateLimits {PerDay = 5000}}
                    //},
                    //white list API keys that don’t require throttling
                    //ClientWhitelist = new List<string> {"admin-key"},

                    //scope to routes (IP + Client Key + Request URL)
                    EndpointThrottling = true,
                    EndpointRules = new Dictionary<string, RateLimits>
                    {
                        {"feedback/", new RateLimits {PerHour = 2}}, // two feedback per hour per client IP
                    }
                },
                Repository = new CacheRepository(),
            };
#if DEBUG
            throttlingHandler.Logger = new TracingThrottleLogger(traceWriter);
#endif
            config.MessageHandlers.Add(throttlingHandler);
        }
    }
}

