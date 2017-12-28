using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;

namespace ACTransit.Framework.Web.Razor
{
    public abstract class TemplateBase
    {
        [Browsable(false)]
        public StringBuilder Buffer { get; set; }

        [Browsable(false)]
        public StringWriter Writer { get; set; }

        public TemplateBase()
        {
            Buffer = new StringBuilder();
            Writer = new StringWriter(Buffer);
        }

        public abstract void Execute();

        public virtual void  Execute(params object[] param)
        {
            Execute();
        }


        // Writes the results of expressions like: "@foo.Bar"
        public virtual void Write(object value)
        {
            // Don't need to do anything special
            // Razor for ASP.Net does HTML encoding here.
            WriteLiteral(value);
        }

        // Writes literals like markup: "<p>Foo</p>"
        public virtual void WriteLiteral(object value)
        {
            Buffer.Append(value);
        }
    }
}
