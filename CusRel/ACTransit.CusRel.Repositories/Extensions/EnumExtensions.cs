using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ACTransit.CusRel.Repositories.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            return attributes.Length > 0 ? attributes[0].Name : value.ToString();
        }

        public static T FromDisplayName<T>(this string Name)
        {
            Name = Name.Trim().ToLower();
            var fis = typeof(T).GetFields();
            foreach (var fi in fis)
            {
                var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
                if (attributes.Length > 0 && attributes[0].Name.ToLower() == Name)
                    return (T)Enum.Parse(typeof(T), fi.Name);
            }
            throw new Exception("Not found");
        }
    }
}
