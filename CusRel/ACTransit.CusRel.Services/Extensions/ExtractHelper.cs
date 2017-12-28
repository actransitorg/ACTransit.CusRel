using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text.RegularExpressions;
using ACTransit.Contracts.Data.CusRel.TicketContract.Result;
using ACTransit.Framework.Extensions;

namespace ACTransit.CusRel.Services.Extensions
{
    public static class ExtractHelper
    {
        /// <summary>
        /// Recursively get all properties from object graph for dynamic ticket search criteria feature.
        /// Data annotation attributes are used for excluding [NotMapped], exact description [Description] or 'managed' description [Display].
        /// </summary>
        /// <param name="o"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static IEnumerable<TicketSearchField> Flatten(object o, string parent = null, string parentName = null)
        {
            var t = o.GetType();
            var props = t.GetProperties();
            foreach (var prp in props)
            {
                if (prp.PropertyType.Module.ScopeName != "CommonLanguageRuntimeLibrary")
                {
                    var recursiveValue = prp.GetValue(o);
                    if (recursiveValue == null) continue;
                    if (prp.PropertyType.GetProperties().Length == 0)
                        yield return ParseProperty(prp, o, parentName);
                    foreach (var fields in Flatten(recursiveValue, t.Name, parentName != null ? parentName + "." + prp.Name : prp.Name))
                        yield return fields;
                }
                else
                {
                    var result = ParseProperty(prp, o, parentName);
                    if (result != null)
                        yield return result;
                }
            }
        }

        private static TicketSearchField ParseProperty(PropertyInfo prp, object o, string parentName = null)
        {
            var notMapped = (NotMappedAttribute[])prp.GetCustomAttributes(typeof(NotMappedAttribute), false);
            if (notMapped.Length > 0) return null;
            object value;
            try { value = prp.GetValue(o); }
            catch { value = null; }
            var stringValue = (value != null) ? value.ToString() : "";
            var displayAttr = (DisplayAttribute[])prp.GetCustomAttributes(typeof(DisplayAttribute), false);
            var descriptionAttr = (DescriptionAttribute[])prp.GetCustomAttributes(typeof(DescriptionAttribute), false);
            var objectGraphRefValue = ("Ticket." + (parentName + "." + prp.Name).Replace("TicketSearchParams.", "")).Replace("..", ".");
            var displayNameValue =
                (descriptionAttr.Length > 0
                ? descriptionAttr[0].Description
                : (parentName + "." + (displayAttr.Length > 0 ? displayAttr[0].Name : prp.Name)).ForDisplay());
            return new TicketSearchField
            {
                Name = displayNameValue,
                ObjectGraphRef = objectGraphRefValue,
                DefaultValue = stringValue
            };
        }

        private static readonly Regex displayRegex = new Regex(@"(\b\S+\s*\b)(?=.{0,15}\1)", RegexOptions.Compiled);

        private static string ForDisplay(this string value)
        {
            value = value.Replace(".", "").Replace("TicketSearchParams", "").PascalCaseToDescription();
            //if (value.StartsWith("User"))
            //    value = "Last Updated By User" + value.Substring(4);
            if (displayRegex.IsMatch(value))
                value = displayRegex.Replace(value, "");
            value = value.Replace("Search Params", "").Replace("  ", " ").Trim();
            if (value.StartsWith("Ticket"))
                value = value.Substring(7);
            return value;
        }
    }
}
