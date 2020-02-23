using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyInvalidLanguagePath : Exception
    {
        public TransmogrifyInvalidLanguagePath(string message)
            : base(message) { }
    }
}