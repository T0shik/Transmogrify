using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyFailedToResolveLanguageCode : Exception
    {
        public TransmogrifyFailedToResolveLanguageCode(string message)
            : base(message) { }
    }
}