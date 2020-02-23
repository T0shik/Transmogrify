using System;
using System.Collections.Generic;
using System.IO;
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

            var primaryPack = new LanguagePack(_library[packName], packName);

            if (HasDefaultLanguage && DefaultLanguage != packName)
            {
                return new TranslationContext(primaryPack, new LanguagePack(_library[DefaultLanguage], packName));
            }
            
            return new TranslationContext(primaryPack);
        }

        private string DefaultLanguage => _transmogrifyConfig.DefaultLanguage;
        private bool HasDefaultLanguage => !string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage);

        private async Task<string> GetLanguagePackName()
        {
            foreach (var languageResolver in _languageResolvers)
            {
                var code = await languageResolver.GetLanguageCode();
                if (!string.IsNullOrWhiteSpace(code) && _library.ContainsKey(code))
                {
                    return code;
                }
            }

            if (string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage))
            {
                throw new
                    TransmogrifyFailedToResolveLanguageCode("Couldn't resolve a language code and no default language was set.");
            }

            return _transmogrifyConfig.DefaultLanguage;
        }
    }
}