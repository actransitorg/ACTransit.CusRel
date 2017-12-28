using System;

namespace ACTransit.Framework.Web.Exceptions
{
    public class FriendlyException : ACTransit.Framework.Exceptions.FriendlyException
    {
        public FriendlyException(FriendlyExceptionType exceptionType): base(GetMessage(exceptionType))
        {
            ExceptionType = exceptionType;
        }

        public FriendlyException(string message)
            : base(message)
        {
            ExceptionType = FriendlyExceptionType.Other;
        }

        public FriendlyExceptionType ExceptionType { get; private set; }

        private static string GetMessage(FriendlyExceptionType exceptionType)
        {
            string message;
            switch (exceptionType)
            {
                case FriendlyExceptionType.PsMismatch:
                    message = "PeopleSoft data has changed. please refresh your page and try again.";
                    break;
                case FriendlyExceptionType.AccessDenied:
                    message = "You don't have enough access privileges for this operation.";
                    break;
                default:
                    message = "Some error occured.";
                    break;
            }
            return message;
        }

    }

    public class FriendlyException<T> : ACTransit.Framework.Exceptions.FriendlyException where T : struct, IConvertible
    {
        public FriendlyException(string message) : base(message)
        {
        }
        public T ExceptionType { get; protected set; }

        public virtual string GetMessage(T exceptionType)
        {
            return "Some error occured.";
        }

       
    }

    public enum FriendlyExceptionType
    {
        Other,
        /// <summary>
        /// PeopleSoft data mismatch
        /// </summary>
        PsMismatch,
        AccessDenied
    }
}