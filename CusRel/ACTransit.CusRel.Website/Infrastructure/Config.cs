using System;
using System.Configuration;

namespace ACTransit.CusRel.Infrastructure
{
    public class Config
    {
        public static readonly bool WindowsAuthenticationOnly = Convert.ToBoolean(ConfigurationManager.AppSettings["CusRel:WindowsAuthenticationOnly"]);
        public static readonly bool IsReferenceMode = Convert.ToBoolean(ConfigurationManager.AppSettings["CusRel:Tokens:IsReferenceMode"]);
        public static readonly int MaxSearchCount = Convert.ToInt32(ConfigurationManager.AppSettings["MaxSearchCount"]);
        public static readonly int MaxCommentsLength = Convert.ToInt32(ConfigurationManager.AppSettings["MaxCommentsLength"]);
        public static readonly string ReverseGeocodeUrl = ConfigurationManager.AppSettings["ReverseGeocodeUrl"];
        public static readonly string Divisions = ConfigurationManager.AppSettings["Divisions"];
    }
}