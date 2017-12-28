namespace ACTransit.CusRel.Repositories.DAL
{

    public class CusRelDbContext : DataAccess.CustomerRelations.CusRelEntities
    {
        public CusRelDbContext(): base("CusRelEntities")
        {
        }

        public static CusRelDbContext Create()
        {
            return new CusRelDbContext();
        }
    }

}
