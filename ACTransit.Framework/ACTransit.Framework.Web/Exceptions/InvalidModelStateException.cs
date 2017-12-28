using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ACTransit.Framework.Web.Exceptions
{
    public class InvalidModelStateException : Exception
    {
        public readonly Dictionary<string, List<string>> ModelErrors = new Dictionary<string, List<string>>();

        public InvalidModelStateException() { }

        public InvalidModelStateException(string message)
            : base(message)
        {
            AddModelError(string.Empty, message);
        }

        public InvalidModelStateException(string message, Exception innerException)
            : base(message, innerException)
        {
            AddModelError(string.Empty, message);
        }

        public string UserFriendlyMessage
        {
            get { return "Invalid Model State"; }
        }

        public void AddModelError(string key, string errorMessage)
        {
            if (!ModelErrors.ContainsKey(key))
                ModelErrors.Add(key, new List<string>());

            ModelErrors[key].Add(errorMessage);
        }

        public void AddToModelState(ModelStateDictionary modelState)
        {
            foreach (var key in ModelErrors.Keys)
            {
                foreach (var errorMessage in ModelErrors[key])
                {
                    modelState.AddModelError(key, errorMessage);
                }
            }
        }
    }
}