using System.Collections.Generic;

namespace Transmogrify {
    public class Config
    {
        public Config()
        {
            LanguagePacks = new Dictionary<string, string>();
        }
        
        public Dictionary<string, string> LanguagePacks { get; }
    }
}