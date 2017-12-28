using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security.AntiXss;
using ACTransit.CusRel.Infrastructure;
using Ganss.XSS;

namespace ACTransit.CusRel.Models.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString GetDisplayName<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression));
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString GetDescription<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var value = metaData.Description ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression));
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString GetReasonsString(this HtmlHelper html, string value)
        {
            return MvcHtmlString.Create(value);
        }

        public static MvcHtmlString GetCommentsString(this HtmlHelper html, string value)
        {
            value = HttpUtility.HtmlDecode(new HtmlSanitizer().Sanitize(value));
            return MvcHtmlString.Create(value == null ? null : TruncateOnWordBoundary(value, Config.MaxCommentsLength).Replace(@"""", "'"));
        }

        public static MvcHtmlString GetMsString(this HtmlHelper html, string value)
        {
            value = HttpUtility.HtmlDecode(new HtmlSanitizer().Sanitize(value));
            return MvcHtmlString.Create(string.IsNullOrEmpty(value) ? "" : "'" + HttpUtility.JavaScriptStringEncode(value) + "'");
        }

        public static MvcHtmlString AsHtmlString(this string text)
        {
            return new MvcHtmlString(HttpUtility.HtmlDecode(new HtmlSanitizer().Sanitize(text)));
        }

        private const string Ellipsis = "...";
 
         /// <summary>
         /// Truncates the supplied content to the max length on a word boundary and adds an ellipsis if longer.
         /// </summary>
         /// <param name="content">The string to truncate</param>
         /// <param name="maxLength">Max number of characters to show, not including continuation content</param>
         /// <returns>The truncated string</returns>
         public static string TruncateOnWordBoundary(this string content, int maxLength)
         {
             return content.TruncateOnWordBoundary(maxLength, Ellipsis);
         }
 
         /// <summary>
         /// Truncates the supplied content to the max length on a word boundary and adds the suffix if longer.
         /// </summary>
         /// <param name="content">The string to truncate</param>
         /// <param name="maxLength">Max number of characters to show, not including the suffix</param>
         /// <param name="suffix">Suffix to append if content is truncated</param>
         /// <returns>The truncated string</returns>
         public static string TruncateOnWordBoundary(this string content, int maxLength, string suffix)
         {
             // No content? Return an empty string.
             if (String.IsNullOrEmpty(content))
                 return String.Empty;
 
             // Content is shorter than the max length? Return the whole string.
             if (content.Length <= maxLength)
                 return content;
 
             // Find the word boundary.
             var i = maxLength;
             while (i > 0)
             {
                 if (Char.IsWhiteSpace(content[i]))
                     break;
                 i--;
             }
 
             // Can't truncate on a word boundary? Just return the suffix, e.g. "...".
             if (i <= 0)
                 return (suffix ?? Ellipsis); // Just in case a null suffix was supplied.
 
             return content.Substring(0, i) + (suffix ?? Ellipsis);
         }

         public static RouteValueDictionary EnabledIf(this object htmlAttributes, bool enabled)
         {
             var attributes = new RouteValueDictionary(htmlAttributes);
             if (!enabled)
                 attributes["disabled"] = "disabled";
             return attributes;
         }

         public static RouteValueDictionary DisabledIf(this object htmlAttributes, bool disabled)
         {
             var attributes = new RouteValueDictionary(htmlAttributes);
             if (disabled)
                 attributes["disabled"] = "disabled";
             return attributes;
         }

         public static bool IsDebug(this HtmlHelper htmlHelper)
         {
            #if DEBUG
                  return true;
            #else
                  return false;
            #endif
         }
    }
}