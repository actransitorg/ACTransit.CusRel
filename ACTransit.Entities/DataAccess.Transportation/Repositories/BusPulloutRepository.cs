using System.Collections.Generic;
using System.Linq;
using ACTransit.Entities.Transportation;

namespace ACTransit.DataAccess.Transportation.Repositories
{
    public class BusPulloutRepository : BaseRepository
    {
        public List<GetBusPullOuts_Result> GetBusPullout(string division=null)
        {
            return ((TransportationEntities) Context).GetBusPullOuts(division).ToList();            
        }
    }
}
