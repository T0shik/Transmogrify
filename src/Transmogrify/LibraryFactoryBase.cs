using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transmogrify.Exceptions;

namespace Transmogrify
{
    public abstract class LibraryFactoryBase : ILibraryFactory
    {
        protected LibraryFactoryBase(TransmogrifyConfig transmogrifyConfig)
        {
            if (string.IsNullOrWhiteSpace(transmogrifyConfig.LanguagePath))
            {
                throw new
                    TransmogrifyInvalidLanguagePathException($"Language path: {transmogrifyConfig.LanguagePath} is not valid");
            }
            
            _languagePath = transmogrifyConfig.LanguagePath;
            _cachedLibrary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            _loaded = false;
        }

        private readonly string _languagePath;
        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _cachedLibrary;
        private bool _loaded;
        
        public abstract T Deserialize<T>(string value);

        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetOrLoad()
        {
            if (!_loaded) LoadLibraryFromFile();
            return _cachedLibrary;
        }
        
        private void LoadLibraryFromFile()
        {
            foreach (var packPath in Directory.EnumerateDirectories(_languagePath))
            {
                var pack = packPath.Split(Path.DirectorySeparatorChar).Last();
                _cachedLibrary[pack] = new Dictionary<string, Dictionary<string, string>>();

                var allFiles = Directory.EnumerateFiles(packPath, "*", SearchOption.AllDirectories).ToList();
                foreach (var filePath in allFiles)
                {
                    var fileName = filePath
                                   .Split(Path.DirectorySeparatorChar).Last()
                                   .Split('.').First();

                    var json = File.ReadAllText(filePath);

                    _cachedLibrary[pack][fileName] = Deserialize<Dictionary<string, string>>(json);
                }
            }

            _loaded = true;
        }
    }
}