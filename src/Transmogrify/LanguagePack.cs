using System.Collections.Generic;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public class LanguagePack : IReader
    {
        private readonly Dictionary<string, Dictionary<string, string>> _languagePack;
        private readonly string _packName;

        public LanguagePack(Dictionary<string, Dictionary<string, string>> languagePack, string packName)
        {
            _languagePack = languagePack;
            _packName = packName;
        }

        public string Read(string page, string phrase)
        {
            if (!_languagePack.ContainsKey(page))
            {
                throw new
                    TransmogrifyMissingKey($"{page} page is missing from the \"{_packName}\" language pack.");
            }

            if (!_languagePack[page].ContainsKey(phrase))
            {
                throw new
                    TransmogrifyMissingKey($"Phrase \"{phrase}\" is missing from the {page} page in {_packName} library");
            }

            return _languagePack[page][phrase];
        }

        public string Read(
            string page,
            string phrase,
            params string[] parameters)
        {
            var translation = Read(page, phrase);

            return string.Format(translation, parameters);
        }
    }
}