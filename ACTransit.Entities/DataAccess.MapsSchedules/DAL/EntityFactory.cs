using ACTransit.Contracts.Data.Common.PublicSite;

namespace ACTransit.DataAccess.MapsSchedules.DAL
{
    public static class EntityFactory
    {
        static EntityFactory() { }

        public static MapsEntities Create(RequestState RequestState = null)
        {
            return RequestState != null && RequestState.ConnectionStrings != null
                ? new MapsEntities(RequestState.ConnectionStrings[typeof(MapsEntities).Name].ToString())
                : new MapsEntities();
        }
    }
}
