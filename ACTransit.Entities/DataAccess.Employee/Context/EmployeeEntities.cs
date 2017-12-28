using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Entities.Employee;

namespace ACTransit.DataAccess.Employee
{
    public partial class EmployeeEntities: DbContext
    {
        public EmployeeEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }
    }

    public class EmployeeDbContextFactory : IDbContextFactory<EmployeeEntities>
    {
        private string _nameOrConnectionString;

        public void SetNameOrConnectionString(string nameOrConnectionString)
        {
            _nameOrConnectionString = nameOrConnectionString;
        }

        public EmployeeEntities Create()
        {
            return _nameOrConnectionString == null ? null : new EmployeeEntities(_nameOrConnectionString);
        }
    }
}
