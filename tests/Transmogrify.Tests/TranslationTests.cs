using System.Collections.Generic;
using System.Threading.Tasks;
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

        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> _library =
            new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                ["en"] = new Dictionary<string, Dictionary<string, string>>
                {
                    ["main"] = new Dictionary<string, string>
                    {
                        ["Hello"] = "Hello World!",
                    },
                    ["second"] = new Dictionary<string, string>
                    {
                        ["Sec"] = "Sec"
                    }
                },
                ["ru"] = new Dictionary<string, Dictionary<string, string>>
                {
                    ["main"] = new Dictionary<string, string>
                    {
                        ["Hello"] = "Привет Мир!",
                    },
                    ["second"] = new Dictionary<string, string>
                    {
                        ["Sec"] = "Сек"
                    }
                }
            };

        public TranslationTests()
        {
            _mockLibraryFactory.Setup(x => x.GetOrLoad())
                               .Returns(_library);
        }

        [Fact]
        public async Task ThrowsWhen_LanguagePackMissingFile()
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("en");
            var languageResolvers = new[] {_mockLanguageResolver.Object};
            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            Task<string> Action() => translator.GetTranslation("doesnt exist", "Hello");

            await ThrowsAsync<TransmogrifyMissingKeyException>(Action);
        } 
        
        [Fact]
        public async Task ThrowsWhen_LanguagePackFileMissingKey()
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("en");
            var languageResolvers = new[] {_mockLanguageResolver.Object};
            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            Task<string> Action() => translator.GetTranslation("main", "doesntexist");

            await ThrowsAsync<TransmogrifyMissingKeyException>(Action);
        } 
        
        [Theory]
        [InlineData("en", "Hello World!")]
        [InlineData("ru", "Привет Мир!")]
        public async Task GetTranslations_FindsValueInGivenFileAtGivenKey(string lang, string result)
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);
            var languageResolvers = new[] {_mockLanguageResolver.Object};
            var translator1 = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            var translation1 = await translator1.GetTranslation("main", "Hello");

            Equal(result, translation1);
        }
        
        [Theory]
        [InlineData("Hello {0}", "Hello One", "One")]
        [InlineData("Hello {0}, {1}", "Hello One, Two", "One", "Two")]
        [InlineData("Hello {0}, {1}, {2}", "Hello One, Two, Three", "One", "Two", "Three")]
        public async Task GetTranslations_InsertsSingleParameter(string sentenceMap, string expected, params string[] parameters)
        {
            _library["en"]["main"]["Param"] = sentenceMap;
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync("en");
            var languageResolvers = new[] {_mockLanguageResolver.Object};
            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            var translation = await translator.GetTranslation("main", "Param", parameters);

            Equal(expected, translation);
        }

        [Theory]
        [InlineData("en", "Hello World!", "Sec")]
        [InlineData("ru", "Привет Мир!", "Сек")]
        public async Task BothFilesAreLoadedInMemory(
            string pack,
            string expected1,
            string expected2)
        {
            _mockLanguageResolver.Setup(x => x.GetLanguageCode()).ReturnsAsync(pack);
            var languageResolvers = new[] {_mockLanguageResolver.Object};
            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            var translation1 = await translator.GetTranslation("main", "Hello");
            var translation2 = await translator.GetTranslation("second", "Sec");

            Equal(expected1, translation1);
            Equal(expected2, translation2);
        }

        [Theory]
        [InlineData("en", "ru", "Hello World!")]
        [InlineData("ru", "en", "Привет Мир!")]
        public async Task LanguageResolversUsedInOrder(
            string first,
            string second,
            string result)
        {
            var languageResolverMock1 = new Mock<ILanguageResolver>();
            var languageResolverMock2 = new Mock<ILanguageResolver>();
            languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(first);
            languageResolverMock2.Setup(x => x.GetLanguageCode()).ReturnsAsync(second);
            var languageResolvers = new[] {languageResolverMock1.Object, languageResolverMock2.Object};

            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            var translation = await translator.GetTranslation("main", "Hello");
            Equal(result, translation);
        }

        [Theory]
        [InlineData("Albanian", "en", "Hello World!")]
        [InlineData("Albanian", "ru", "Привет Мир!")]
        public async Task SkipsMissingLanguageResolvers(
            string first,
            string second,
            string result)
        {
            var languageResolverMock1 = new Mock<ILanguageResolver>();
            var languageResolverMock2 = new Mock<ILanguageResolver>();
            languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(first);
            languageResolverMock2.Setup(x => x.GetLanguageCode()).ReturnsAsync(second);
            var languageResolvers = new[] {languageResolverMock1.Object, languageResolverMock2.Object};

            var translator = new Translator(Config, languageResolvers, _mockLibraryFactory.Object);

            var translation = await translator.GetTranslation("main", "Hello");
            Equal(result, translation);
        }

        // [Theory]
        // [InlineData(c_englishPack, "Bob")]
        // [InlineData(c_russianPack, "Боб")]
        // public async Task NestedObjectNavigationWithinFile(string lang, string expected)
        // {
        //     var languageResolverMock1 = new Mock<ILanguageResolver>();
        //     languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);
        //     
        //     var translator = new Translator(Config, new[] {languageResolverMock1.Object},
        //                                     new System.Text.Json.Transmogrify.TransmogrifyJson());
        //     
        //     var translation = await translator.GetTranslation("main", "Nest:Bob");
        //     Assert.Equal(expected, translation);
        // }
    }
}