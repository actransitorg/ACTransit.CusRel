using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Entities.CustomerRelations;
using ACTransit.Contracts.DataContracts.CusRel.UserContract;
using ACTransit.Contracts.DataContracts.CusRel.UserContract.Result;

namespace ACTransit.DataAccess.CustomerRelations.Repositories
{
    public class CusRelRepository : IDisposable
    {
        private CusRelEntities context;
        public string ErrorMessage { get; private set; }

        //public int CurrentVersionId { get; private set; }

        public CusRelRepository(CusRelEntities context)
        {
            this.context = context;
            InitCusRelContext();
        }

        private void InitCusRelContext()
        {
            if (context == null)
                context = new CusRelEntities();
        }

        // =============================================================

        #region Bookkeeping

        public int SaveChanges()
        {
            if (context != null)
                return context.SaveChanges();
            return 0;
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                context.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Save Entities

        public int AddOrUpdateUser(AuthorizedUsers User)
        {
            context.AuthorizedUsers.AddOrUpdate(User);
            return SaveChanges();
        }

        #endregion

    }

}
