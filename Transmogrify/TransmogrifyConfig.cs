using System;
using System.Collections.Generic;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class TransmogrifyConfig
    {
        private readonly List<Type> _languageResolvers;

        public TransmogrifyConfig()
        {
            _languageResolvers = new List<Type>();
            LanguagePacks = new Dictionary<string, string>();
        }

        public IEnumerable<Type> LanguageResolvers => _languageResolvers;
        public Dictionary<string, string> LanguagePacks { get; }
        public string DefaultLanguage { get; set; }

        public void AddResolver(Type languageResolver)
        {
            if (!typeof(ILanguageResolver).IsAssignableFrom(languageResolver))
            {
                throw new
                    TransmogrifyInvalidLanguageResolverType($"Expected type: {languageResolver} to implement ILanguageResolver interface.");
            }

            _languageResolvers.Add(languageResolver);
        }
    }
}