using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class Translator : ITranslator
    {
        private readonly TransmogrifyConfig _transmogrifyConfig;
        private readonly IEnumerable<ILanguageResolver> _languageResolvers;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _library;

        public Translator(
            TransmogrifyConfig transmogrifyConfig,
            IEnumerable<ILanguageResolver> languageResolvers,
            ILibraryFactory libraryFactory)
        {
            _transmogrifyConfig = transmogrifyConfig;
            _languageResolvers = languageResolvers;
            _library = libraryFactory.GetOrLoad();
        }

        public async Task<string> GetTranslation(string file, string key)
        {
            var code = await GetLanguageCode();

            if (!_library[code].ContainsKey(file))
            {
                var originalCode = code;
                if (!string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage))
                    code = _transmogrifyConfig.DefaultLanguage;
                else
                    throw new
                        TransmogrifyMissingKeyException($"File: \"{file}\" is missing from the library: \"{code}\"");

                if (!_library[code].ContainsKey(file))
                    throw new
                        TransmogrifyMissingKeyException($"File: \"{file}\" is missing from the library: \"{originalCode}\" and \"{code}\"");
            }

            if (!_library[code][file].ContainsKey(key))
            {
                var originalCode = code;
                if (!string.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage))
                    code = _transmogrifyConfig.DefaultLanguage;
                else
                    throw new
                        TransmogrifyMissingKeyException($"Key: \"{key}\" is missing from the library: \"{code}\" file: \"{file}\"");

                if (!_library[code].ContainsKey(file))
                    throw new
                        TransmogrifyMissingKeyException($"Key: \"{key}\" is missing from the library: \"{originalCode}\" and \"{code}\" file: \"{file}\"");
            }

            return _library[code][file][key];
        }

        public async Task<string> GetTranslation(
            string file,
            string key,
            params string[] parameters)
        {
            var translation = await GetTranslation(file, key);

            return string.Format(translation, parameters);
        }

        private async Task<string> GetLanguageCode()
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