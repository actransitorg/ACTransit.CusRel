using System.Collections.Generic;
using System.Linq;
using ACTransit.Entities.Transportation;

namespace ACTransit.DataAccess.Transportation.Repositories
{
    public class BookingRepository : BaseRepository
    {
        public List<Booking> GetBookings()
        {
            return ((TransportationEntities) Context).Bookings.ToList();
        }
    }
}