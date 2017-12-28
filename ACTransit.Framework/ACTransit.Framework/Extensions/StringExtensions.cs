using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace ACTransit.Framework.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a string to a double
        /// </summary>
        /// <returns>A double value, or null when it cannot be converted.</returns>
        public static double? ToDouble( this string str )
        {
            double doubleValue;

            return double.TryParse( str, out doubleValue )
                ? (double?)doubleValue
                : null;
        }

        /// <summary>
        /// Convert a string to an integer
        /// </summary>
        /// <returns>An integer value, or null when cannot be converted.</returns>
        public static int? ToInt( this string str )
        {
            int intValue;

            return int.TryParse( str, out intValue )
                ? (int?)intValue
                : null;
        }

        /// <summary>
        /// Convert a string to a decimal
        /// </summary>
        /// <returns>A decimal value, or null when cannot be converted.</returns>
        public static decimal? ToDecimal( this string str )
        {
            decimal decimalValue;

            return decimal.TryParse( str, out decimalValue )
                ? (decimal?)decimalValue
                : null;
        }

        /// <summary>
        /// Convert a string to a long
        /// </summary>
        /// <returns>A long value, or null when cannot be converted.</returns>
        public static long? ToLong( this string str )
        {
            long longValue;

            return long.TryParse( str, out longValue )
                ? (long?)longValue
                : null;
        }

        /// <summary>
        /// Convert a string to a datetime
        /// </summary>
        /// <returns>A datetime value, or null when cannot be converted.</returns>
        public static DateTime? ToDateTime( this string str )
        {
            DateTime dateValue;

            return DateTime.TryParse( str, out dateValue )
                ? (DateTime?)dateValue
                : null;
        }

        /// <summary>
        /// Convert a string to a bool
        /// </summary>
        /// <returns>A bool value, or null when cannot be converted.</returns>
        public static bool? ToBool( this string str )
        {
            bool boolValue;

            return bool.TryParse( str, out boolValue )
                ? (bool?)boolValue
                : null;
        }

        /// <summary>
        /// Convert a string to an object of any type. 
        /// </summary>
        /// <returns>Casted value, or the default value for requested type when cast fails</returns>
        public static T Cast<T>( this string str )
        {
            try
            {
                return (T)Convert.ChangeType( str, typeof( T ) );
            }
            catch( Exception )
            {
                return default( T );
            }
        }

        /// <summary>
        /// Return a list of values converted to type T
        /// </summary>
        /// <returns>an IEnumerable of type T</returns>
        public static IEnumerable<T> ToEnumerable<T>( this string str, string seperator = "," )
        {
            return str.Split( new[] { seperator }, StringSplitOptions.RemoveEmptyEntries ).OfType<T>();
        }

        /// <summary>
        /// Tries to convert a string into a Dictionaty
        /// </summary>
        /// <typeparam name="TKey">Key data type</typeparam>
        /// <typeparam name="TValue">Value data type</typeparam>
        /// <returns>Returnes a Dictionary of the desired type.</returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>( this string str, string keyValueSeperator = "|", string lineSeperator = "," )
        {
            var lineValues = str.Split( new[] { lineSeperator }, StringSplitOptions.RemoveEmptyEntries );

            return lineValues.Select( line =>
            {
                var keyValuePair = line.Split( new[] { keyValueSeperator }, StringSplitOptions.RemoveEmptyEntries );

                return keyValuePair.Length == 2
                    ? new KeyValuePair<TKey, TValue>( str.Cast<TKey>(), str.Cast<TValue>() )
                    : new KeyValuePair<TKey, TValue>( default( TKey ), default( TValue ) );
            } )
            .ToDictionary( kvp => kvp.Key, kvp => kvp.Value );
        }

        /// <summary>
        /// Tries to convert a string into a Dictionaty
        /// </summary>
        /// <typeparam name="TKey">Key data type</typeparam>
        /// <typeparam name="TValue">Value data type</typeparam>
        /// <returns>Returnes a Dictionary of the desired type.</returns>
        public static IDictionary<TKey, TValue> ToDictionaryTrim<TKey, TValue>(this string str, string keyValueSeperator = "|", string lineSeperator = ",")
        {
            var lineValues = str.Split(new[] { lineSeperator }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

            var result = lineValues.Select(line =>
            {
                var keyValuePair = line.Split(new[] { keyValueSeperator }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();            

                return keyValuePair.Length == 2
                    ? new KeyValuePair<TKey, TValue>(keyValuePair[0].Cast<TKey>(), keyValuePair[1].Cast<TValue>())
                    : new KeyValuePair<TKey, TValue>(default(TKey), default(TValue));
            });
            return result.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Will add ellipses to the given string if it exceeds a specific length.  
        /// </summary>
        /// <param name="str">The string</param>
        /// <param name="maxCharacters">Max number of characters before adding ellipses.</param>
        /// <param name="ellipsesText">The text of ellipses to use.</param>
        /// <returns>Example: "Long str..."</returns>
        public static string Ellipses( this string str, int maxCharacters, string ellipsesText = "..." )
        {
            var length = str.Length;

            return length > maxCharacters
                ? string.Format( "{0}{1}", str.Substring( 0, maxCharacters ), ellipsesText )
                : str;
        }

        public static string PascalCaseToDescription(this string str)
        {
            return str == null ? null : Regex.Replace(str, "(\\B[A-Z])", " $1");
        }

        public static string DescriptionToPascalCase(this string str)
        {
            var result = str.Replace(" ", "");
            return result.PascalCaseToDescription() == str ? result : str;
        }

        public static string NullableTrim(this string value)
        {
            return value == null ? null : value.Trim();
        }
        public static string IsNull(this string value, string defaultValue)
        {
            return value ?? defaultValue;
        }

        public static string TruncateLongString(this string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static bool IsNumeric(this string input)
        {
            int test;
            return (!string.IsNullOrWhiteSpace(input)) && int.TryParse(input, out test);            
        }

        public static string NullOrWhiteSpaceTrim(this string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        public static string[] ParseCsv(this string line, int start=0, char delimiter=',')
        {
            const char quote = '"';

            var isInside = false;

            var values = new List<string>();
            string currentStr = "";
            if (line != null && line.Length > start)
            {
                for (int i = start; i < line.Length; i++)
                {
                    if (line[i] == quote)
                    {
                        if (isInside)
                        {
                            if (line.Length > i + 1 && line[i + 1] == quote)
                            {
                                currentStr += quote;
                                i++;
                            }
                            else
                                isInside = false;
                        }
                        else
                            isInside = true;
                    }
                    else if (line[i] == delimiter && !isInside)
                    {
                        values.Add(currentStr);
                        currentStr = "";
                    }
                    else
                        currentStr += line[i];
                }
                values.Add(currentStr);
            }

            return values.ToArray();
        }
    }
}