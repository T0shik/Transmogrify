using System;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class TranslationContext : IReader
    {
        private readonly IReader _primary;
        private readonly IReader _fallback;

        public TranslationContext(IReader primary)
        {
            _primary = primary;
        }

        public TranslationContext(IReader primary, IReader fallback)
        {
            _primary = primary;
            _fallback = fallback;
        }

        public string Read(string page, string phrase)
        {
            return ReadWithFallback(pack => pack.Read(page, phrase));
        }

        public string Read(
            string page,
            string phrase,
            params string[] parameters)
        {
            return ReadWithFallback(pack => pack.Read(page, phrase, parameters));
        }

        private string ReadWithFallback(Func<IReader, string> read)
        {
            try
            {
                return read(_primary);
            }
            catch (TransmogrifyMissingKey)
            {
                if (_fallback == null)
                {
                    throw;
                }

                return read(_fallback); 
            }
        }
    }
}