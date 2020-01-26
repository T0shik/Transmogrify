using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyMissingKeyException : Exception
    {
        public TransmogrifyMissingKeyException(string message)
            : base(message) { }
    }
}