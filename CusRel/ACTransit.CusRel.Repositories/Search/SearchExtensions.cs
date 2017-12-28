using System.Collections.Generic;
using ACTransit.Contracts.Data.CusRel.Common;
using ACTransit.Contracts.Data.CusRel.TicketContract;
using ACTransit.Contracts.Data.CusRel.TicketContract.Params;
using ACTransit.CusRel.Repositories.Mapping;

namespace ACTransit.CusRel.Repositories.Search
{
    public static class SearchExtensions
    {
        public static List<Ticket> Search(this TicketSearchParams searchCriteria, Contracts.Data.CusRel.Common.RequestState RequestState)
        {
            var contact = searchCriteria.CreateSearchContact();
            return contact != null ? contact.GetTickets(null, RequestState) : null;
        }
    }
}