using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Transmogrify {
    public class Translator : ITranslator
    {
        private readonly Config _config;
        private readonly ILanguageResolver _languageResolver;
        private readonly ITransmogrifyJson _transmogrifyJson;
        private readonly Dictionary<string, Dictionary<string, string>> _library;

        public Translator(
            Config config, 
            ILanguageResolver languageResolver,
            ITransmogrifyJson transmogrifyJson)
        {
            _config = config;
            _languageResolver = languageResolver;
            _transmogrifyJson = transmogrifyJson;
            _library = new Dictionary<string, Dictionary<string, string>>();
            
            LoadAllPacks();
        }

        public async Task<string> GetTranslation(string key) => _library[await _languageResolver.GetLanguageCode()][key];

        private void LoadAllPacks()
        {
            foreach (var pack in _config.LanguagePacks)
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