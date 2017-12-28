using System;

namespace ACTransit.Framework.Infrastructure
{
    /// <summary>
    /// Provide the microsoft recommended Disposable pattern implementation
    /// http://msdn.microsoft.com/en-us/library/b1yfkh5e%28v=vs.110%29.aspx
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        protected bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;
        }
    }
}