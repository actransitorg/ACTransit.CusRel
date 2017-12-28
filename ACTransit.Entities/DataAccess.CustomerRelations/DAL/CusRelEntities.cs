using System.Data.Entity;

namespace ACTransit.DataAccess.CustomerRelations // leave namespace as is
{
    public partial class CusRelEntities
    {
        public CusRelEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }
    }
}