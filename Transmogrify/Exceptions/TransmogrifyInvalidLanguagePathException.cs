using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyInvalidLanguagePathException : Exception
    {
        public TransmogrifyInvalidLanguagePathException(string message)
            : base(message) { }
    }
}