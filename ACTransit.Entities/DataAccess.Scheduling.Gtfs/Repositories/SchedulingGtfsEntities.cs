using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTransit.DataAccess.Scheduling.Gtfs
{
    public partial class SchedulingGtfsEntities: DbContext
    {
        public SchedulingGtfsEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}
