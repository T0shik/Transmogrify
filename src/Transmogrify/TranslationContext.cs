using System;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class TranslationContext : IDictionary
    {
        private readonly IDictionary _primary;
        private readonly IDictionary _fallback;

        public TranslationContext(IDictionary primary)
        {
            _primary = primary;
        }

        public TranslationContext(IDictionary primary, IDictionary fallback)
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

        private string ReadWithFallback(Func<IDictionary, string> read)
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