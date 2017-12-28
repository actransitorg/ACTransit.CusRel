using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace ACTransit.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T FromDescription<T>(this string description)
        {
            description = description.Trim();
            var fis = typeof(T).GetFields();
            foreach (var fi in fis)
            {
                var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0 && attributes[0].Description == description)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }
            throw new Exception("Not found");
        }

        public static TEnum EnumParse<TEnum>(this string value, TEnum defaultValue)
        {
            return value != null && Enum.IsDefined(typeof(TEnum), value)
                ? (TEnum)Enum.Parse(typeof(TEnum), value)
                : defaultValue;
        }

        public static Dictionary<string, int> ToDictionary(this Enum @enum, bool asDescription = false)
        {
            var type = @enum.GetType();
            return Enum.GetValues(type).Cast<int>().ToDictionary(e => asDescription ? Enum.GetName(type, e).PascalCaseToDescription() : Enum.GetName(type, e), e => e);
        }

        public static TAttribute GetSingleAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }
}
