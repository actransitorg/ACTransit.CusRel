using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACTransit.DataAccess.CustomerRelations
{
    public class CusRelRepository: IDisposable
    {
        private readonly CusRelEntities cusRelContext;
        private bool disposed;

        #region Constructors / Initialization

        public CusRelRepository() { }

        public CusRelRepository(CusRelEntities context)
        {
            cusRelContext = context;
        }

        #endregion

        // =============================================================

        #region Save/Dispose

        public int SaveChanges()
        {
            return cusRelContext != null ? cusRelContext.SaveChanges() : 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (cusRelContext != null)
                    cusRelContext.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Public Methods

        #endregion

        // =============================================================


    }
}
