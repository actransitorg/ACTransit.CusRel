using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Web.Mvc;
using ACTransit.CusRel.Models.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ACTransit.CusRel.Models.Json
{
    /// <summary>
    /// Switch out built in JavascriptSerializer.Deserialize(T) with JSON.NET deserializer. Used with Action parameters.
    /// TODO: not yet working due to missing action parameter type information (aka what to deserialize into), and ExpandoObject isn't good enough.
    /// </summary>
    public sealed class JsonDotNetValueProviderFactory : ValueProviderFactory
    {
        public JsonSerializerSettings Settings { get; private set; }

        public JsonDotNetValueProviderFactory()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            Settings.Converters.Add(new ExpandoObjectConverter());
        }

        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if ((!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                && (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase)))
                return null;

            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            var bodyText = reader.ReadToEnd();

            if (controllerContext.HttpContext.Request.ContentType.StartsWith("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
            {
                bodyText = JsonConvert.SerializeObject(QueryStringHelper.QueryStringToDict(bodyText));
            }

            var result = !String.IsNullOrEmpty(bodyText)
                ? new DictionaryValueProvider<object>(JsonConvert.DeserializeObject<ExpandoObject>(bodyText, Settings), CultureInfo.CurrentCulture)
                : null;
            return result;
        }
    }
}