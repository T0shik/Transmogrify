using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyMissingKey : Exception
    {
        public TransmogrifyMissingKey(string message)
            : base(message) { }
    }
}