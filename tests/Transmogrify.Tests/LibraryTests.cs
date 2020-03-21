using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;
using Moq;
using Transmogrify.Exceptions;
using Xunit;
using static Xunit.Assert;

namespace Transmogrify.Tests
{
    public class TranslationTests
    {
        private static TransmogrifyConfig Config =>
            new TransmogrifyConfig
            {
                LanguagePath = "./lang"
            };

        private readonly Mock<ILanguageResolver> _mockLanguageResolver = new Mock<ILanguageResolver>();
        private readonly Mock<ILibraryFactory> _mockLibraryFactory = new Mock<ILibraryFactory>();

        private readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> _library =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                ["en"] = new Dictionary<string, Dictionary<string, string>>
                {
                    ["main"] = new Dictionary<string, string>
                    {
                        ["Hello"] = "Hello World!",
                        ["Format"] = "{0}",
                        ["unique"] = "Unique"
                    },
                    ["unique"] = new Dictionary<string, string>
                    {
                        ["unique"] = "Unique"
                    }
                },
                ["ru"] = new Dictionary<string, Dictionary<string, string>>
                {
                    ["main"] = new Dictionary<string, string>
                    {
                        ["Hello"] = "Привет Мир!",
                    },
                }
            };

        private Library CreateLibrary(TransmogrifyConfig config = null)
        {
            return CreateLibrary(new[] {_mockLanguageResolver.Object}, config ?? Config);
        }

        private Library CreateLibrary(IEnumerable<ILanguageResolver> resolvers, TransmogrifyConfig config = null)
        {
            _mockLibraryFactory.Setup(x => x.GetOrLoad()).Returns(_library);
            return new Library(config ?? Config, resolvers, _mockLibraryFactory.Object);
        }

        [Theory]
        [InlineData("en", "Hello World!")]
        [InlineData("en-US;en", "Hello World!")]
        [InlineData("ru", "Привет Мир!")]
        public async Task ResolvesCorrectContext(string language, string expected)
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync(language);
            var context = await CreateLibrary().GetContext();

            var translation = context.Read("main", "Hello");

            Equal(expected, translation);
        }

        [Theory]
        [InlineData("en", "Hello World!")]
        [InlineData("ru", "Привет Мир!")]
        public void ResolvesCorrectSpecificContext(string language, string expected)
        {
            var context = CreateLibrary().GetContext(language);

            var translation = context.Read("main", "Hello");

            Equal(expected, translation);
        }

        [Theory]
        [InlineData("en", "ru", "Hello World!")]
        [InlineData("ru", "en", "Привет Мир!")]
        [InlineData("el", "en", "Hello World!")]
        public async Task ResolversUsedInOrder(
            string firstResult,
            string secondResult,
            string expected)
        {
            var first = new Mock<ILanguageResolver>();
            var second = new Mock<ILanguageResolver>();
            first.Setup(x => x.GetLanguageCode()).ReturnsAsync(firstResult);
            second.Setup(x => x.GetLanguageCode()).ReturnsAsync(secondResult);
            var resolvers = new[] {first.Object, second.Object};
            var context = await CreateLibrary(resolvers).GetContext();

            var translation = context.Read("main", "Hello");

            Equal(expected, translation);
        }

        [Fact]
        public async Task FallsBackWhenPageOrPhraseNotFound()
        {
            var config = Config;
            config.DefaultLanguage = "en";
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("ru");
            var context = await CreateLibrary(config).GetContext();

            var phraseTranslation = context.Read("main", "unique");
            var pageTranslation = context.Read("unique", "unique");

            Equal("Unique", phraseTranslation);
            Equal("Unique", pageTranslation);
        }

        [Theory]
        [InlineData("ru")]
        [InlineData(null)]
        public void FallsBackWhenManualLanguageIsWrong(string lang)
        {
            var config = Config;
            config.DefaultLanguage = "en";
            var context = CreateLibrary(config).GetContext(lang);

            var phraseTranslation = context.Read("main", "unique");
            var pageTranslation = context.Read("unique", "unique");

            Equal("Unique", phraseTranslation);
            Equal("Unique", pageTranslation);
        }

        [Fact]
        public Task Throws_WhenNoLanguageCodeCouldBeResolved()
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("el");
            return ThrowsAsync<TransmogrifyFailedToResolveLanguageCode>(() => CreateLibrary().GetContext());
        }

        [Fact]
        public async Task ResolvesDefaultLanguageWhenPresent()
        {
            var config = Config;
            config.DefaultLanguage = "en";
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("el");
            var context = await CreateLibrary(config).GetContext();

            Equal("Hello World!", context.Read("main", "Hello"));
        }

        [Fact]
        public async Task FormatsPhrase()
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("en");
            var context = await CreateLibrary().GetContext();

            const string word = "Bob";
            var phrase = context.Read("main", "Format", word);
            Equal(word, phrase);
        }

        [Fact]
        public async Task Throws_WhenMissingPhraseAndNoFallback()
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("en");
            var context = await CreateLibrary().GetContext();

            Throws<TransmogrifyMissingKey>(() => context.Read("404", "404"));
        }
    }
}