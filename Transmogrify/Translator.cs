﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class Translator : ITranslator
    {
        private readonly TransmogrifyConfig _transmogrifyConfig;
        private readonly IEnumerable<ILanguageResolver> _languageResolvers;
        private readonly ITransmogrifyJson _transmogrifyJson;
        private readonly Dictionary<string, Dictionary<string, string>> _library;

        public Translator(
            TransmogrifyConfig transmogrifyConfig,
            IEnumerable<ILanguageResolver> languageResolvers,
            ITransmogrifyJson transmogrifyJson)
        {
            _transmogrifyConfig = transmogrifyConfig;
            _languageResolvers = languageResolvers;
            _transmogrifyJson = transmogrifyJson;
            _library = new Dictionary<string, Dictionary<string, string>>();

            LoadAllPacks();
        }

        public async Task<string> GetTranslation(string key)
        {
            var code= await GetLanguageCode();

            if (!_library[code].ContainsKey(key))
                throw new
                    TransmogrifyMissingKeyException($"Key: \"{key}\" is missing from the library: \"{code}\"");

            return _library[code][key];
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

            if (String.IsNullOrEmpty(_transmogrifyConfig.DefaultLanguage))
            {
                throw new
                    TransmogrifyFailedToResolveLanguageCode("Couldn't resolve a language code and no default language was set.");
            }

            return _transmogrifyConfig.DefaultLanguage;
        } 
        
        private void LoadAllPacks()
        {
            foreach (var pack in _transmogrifyConfig.LanguagePacks)
            {
                LoadPack(pack.Key, pack.Value);
            }
        }

        private void LoadPack(string lang, string path)
        {
            var json = File.ReadAllText(path);
            _library[lang] = _transmogrifyJson.Deserialize<Dictionary<string, string>>(json);
        }
    }
}