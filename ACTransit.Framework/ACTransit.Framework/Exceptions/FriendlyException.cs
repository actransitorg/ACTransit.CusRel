using System;

namespace ACTransit.Framework.Exceptions
{
    public class FriendlyException : Exception
    {
        public FriendlyException(string message): base(message){}

    }


}