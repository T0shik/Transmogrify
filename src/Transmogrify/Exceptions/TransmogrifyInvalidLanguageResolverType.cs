using System;

namespace Transmogrify.Exceptions
{
    public class TransmogrifyInvalidLanguageResolverType : Exception
    {
        public TransmogrifyInvalidLanguageResolverType(string message)
            : base(message) { }
    }
}