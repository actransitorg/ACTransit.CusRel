using System;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.DataAccess.MapsSchedules.DAL;

namespace ACTransit.DataAccess.MapsSchedules.Repositories
{
    public static class RepositoryFactory
    {
        static RepositoryFactory() { }

        public static LinesRepository Create(RequestState requestState)
        {
            return new LinesRepository(EntityFactory.Create(requestState));
        }
    }
}
