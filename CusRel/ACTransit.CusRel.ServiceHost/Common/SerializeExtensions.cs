using System.IO;
using System.Xml.Serialization;

namespace ACTransit.CusRel.ServiceHost.Common
{
    public static class SerializeExtensions
    {
        public static void ToSerializedObjectForDebugging(this object o, FileInfo saveTo)
        {
            var t = o.GetType();
            var s = new XmlSerializer(t);
            using (var fs = saveTo.Create())
                s.Serialize(fs, o);
        }

        public static void ToSerializedObjectForDebugging(this object o, string saveTo)
        {
            ToSerializedObjectForDebugging(o, new FileInfo(saveTo));
        }
    }
}
