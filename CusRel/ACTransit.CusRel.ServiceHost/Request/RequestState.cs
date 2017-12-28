using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTransit.CusRel.ServiceHost.Request
{
    public class RequestState
    {
        public string Client;
        public object Data;
        public DateTime? LastRunDate;
        public DateTime? RunDate;
    }
}
