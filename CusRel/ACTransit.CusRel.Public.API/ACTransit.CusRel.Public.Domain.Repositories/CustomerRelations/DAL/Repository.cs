using ACTransit.Contracts.Data.Common.PublicSite;

namespace ACTransit.CusRel.Public.Domain.Repositories.CustomerRelations.DAL
{
    public sealed class Repository
    {
        static Repository() { }

        private Repository() { }

        public static CusRelRepository Context(RequestState RequestState)
        {
            return new CusRelRepository(Entity.Context(RequestState));
        }
    }
}