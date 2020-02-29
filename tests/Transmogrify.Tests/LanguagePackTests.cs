using System.Collections.Generic;
using Transmogrify.Exceptions;
using Xunit;

namespace Transmogrify.Tests
{
    public class LanguagePackTests
    {
        private const string c_phrase = "Hello World!"; 
        private readonly Dictionary<string, Dictionary<string, string>> _pack =
            new Dictionary<string, Dictionary<string, string>>
            {
                ["page"] = new Dictionary<string, string>
                {
                    ["phrase"] = c_phrase,
                    ["format"] = "Hello {0}!",
                }
            };

        [Fact]
        public void ThrowsWhenPageMissing()
        {
            var pack = new LanguagePack(_pack, "test");

            Assert.Throws<TransmogrifyMissingKey>(() => pack.Read("a", "b"));
        }
        
        [Fact]
        public void ThrowsWhenPhraseMissing()
        {
            var pack = new LanguagePack(_pack, "test");

            Assert.Throws<TransmogrifyMissingKey>(() => pack.Read("page", "b"));
        }
        
        [Fact]
        public void ReturnsPhrase()
        {
            var pack = new LanguagePack(_pack, "test");
            var phrase = pack.Read("page", "phrase");
            
            Assert.Equal(c_phrase, phrase);
        }
        
        [Fact]
        public void FormatsPhrase()
        {
            var pack = new LanguagePack(_pack, "test");
            var phrase = pack.Read("page", "format", "World");
            
            Assert.Equal(c_phrase, phrase);
        }
    }
}