using System;

namespace ACTransit.Framework.DataAccess
{
    [Serializable]
    public class RepositoryException : Exception
    {
        public RepositoryException() { }

        public RepositoryException(string message) : base(message) { }

        public RepositoryException(string message, Exception innerException) : base (message, innerException) { }
    }
}