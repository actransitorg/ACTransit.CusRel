using System;

namespace ACTransit.CusRel.Public.API.Tests
{
    public partial class ErrorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            throw new Exception("Errored");
        }
    }
}