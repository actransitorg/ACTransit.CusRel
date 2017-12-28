using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ACTransit.Entities.Transportation;

namespace ACTransit.DataAccess.Transportation.Repositories
{
    public class StopRepository : BaseRepository
    {
        public List<StopDistrict> GetStopDistricts()
        {
            return ((TransportationEntities) Context).StopDistricts.ToList();
        }

        public List<StopDistrict> SearchStopDistricts(Expression<Func<StopDistrict, bool>> predicate)
        {
            return ((TransportationEntities) Context).StopDistricts.Where(predicate).ToList();
        }
    }
}