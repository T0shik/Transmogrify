﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class Library
    {
        private readonly TransmogrifyConfig _transmogrifyConfig;
        private readonly IEnumerable<ILanguageResolver> _languageResolvers;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _library;

        public Library(
            TransmogrifyConfig transmogrifyConfig,
            IEnumerable<ILanguageResolver> languageResolvers,
            ILibraryFactory libraryFactory)
        {
            _transmogrifyConfig = transmogrifyConfig;
            _languageResolvers = languageResolvers;
            _library = libraryFactory.GetOrLoad();
        }

        public async Task<TranslationContext> GetContext()
        {
            var packName = await GetLanguagePackName();

            var primaryPack = CreatePack(packName);

            if (HasDefaultLanguage && DefaultLanguage != packName)
            {
                return new TranslationContext(primaryPack, DefaultPack);
            }

            return new TranslationContext(primaryPack);
        }

        public TranslationContext GetContext(string language)
        {
            if (language == null && HasDefaultLanguage)
            {
                return new TranslationContext(DefaultPack);
            }

            var primaryPack = CreatePack(language);

            if (HasDefaultLanguage && DefaultLanguage != language)
            {
                return new TranslationContext(primaryPack, DefaultPack);
            }

            return new TranslationContext(primaryPack);
        }

        private string DefaultLanguage => _transmogrifyConfig.DefaultLanguage;
        private bool HasDefaultLanguage => !string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage);
        private LanguagePack DefaultPack => CreatePack(DefaultLanguage);
        private LanguagePack CreatePack(string language) => new LanguagePack(_library[language], language);

        private async Task<string> GetLanguagePackName()
        {
            foreach (var languageResolver in _languageResolvers)
            {
                var code = await languageResolver.GetLanguageCode();
                if (string.IsNullOrWhiteSpace(code)) continue;
                
                var pack = LanguagePacks.FirstOrDefault(x => code.Contains(x));
                
                if (!string.IsNullOrEmpty(pack))
                {
                    return pack;
                }
            }

            if (string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage))
            {
                throw new
                    TransmogrifyFailedToResolveLanguageCode("Couldn't resolve a language code and no default language was set.");
            }

            return _transmogrifyConfig.DefaultLanguage;
        }
        private IEnumerable<string> LanguagePacks => _library.Select(x => x.Key);
    }
}