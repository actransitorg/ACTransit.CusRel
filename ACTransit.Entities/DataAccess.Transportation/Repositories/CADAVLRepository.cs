using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Transportation;

namespace ACTransit.DataAccess.Transportation.Repositories
{
    public class CADAVLRepository : BaseRepository
    {
        public List<EmployeeInterface> GetEmployees()
        {
            return ((TransportationEntities) Context).EmployeeInterfaces.ToList();
        }
    }
}