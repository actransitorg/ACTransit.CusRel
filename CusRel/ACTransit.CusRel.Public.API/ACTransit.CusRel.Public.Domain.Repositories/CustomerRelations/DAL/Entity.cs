using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.DataAccess.CustomerRelations;

namespace ACTransit.CusRel.Public.Domain.Repositories.CustomerRelations.DAL
{
    public sealed class Entity
    {
        static Entity() { }

        private Entity() { }

        public static CusRelEntities Context(RequestState RequestState = null)
        {
            return RequestState != null && RequestState.ConnectionStrings != null
                ? new CusRelEntities(RequestState.ConnectionStrings[typeof(CusRelEntities).Name].ToString())
                : new CusRelEntities();
        }
    }
}
