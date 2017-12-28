using System;
using ACTransit.Contracts.Data.Schedules.PublicSite;

namespace ACTransit.DataAccess.MapsSchedules.DAL
{
    /// <summary>
    /// idea from: http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    /// </summary>
    public class LineUnitOfWork : IDisposable
    {
        private readonly MapsEntities context = new MapsEntities();
        private GenericRepository<Line> lineRepository;

        public GenericRepository<Line> LineRepository
        {
            get { return lineRepository ?? (lineRepository = new GenericRepository<Line>(context)); }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
