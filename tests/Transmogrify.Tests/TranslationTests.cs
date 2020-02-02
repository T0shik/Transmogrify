using System.Threading.Tasks;
using Moq;
using Transmogrify.Exceptions;
using Xunit;

namespace Transmogrify.Tests
{
    public class TranslationTests
    {
        private static TransmogrifyConfig Config =>
            new TransmogrifyConfig
            {
                LanguagePath = "./lang"
            };

        [Fact]
        public void ThrowIfLanguagePathIsNullOrEmpty()
        {
            var config = Config;
            config.LanguagePath = "";
            var languageResolverMock = new Mock<ILanguageResolver>();
            Assert.Throws<TransmogrifyInvalidLanguagePathException>(() => new Translator(config,
                                                                                         new[]
                                                                                         {
                                                                                             languageResolverMock.Object
                                                                                         },
                                                                                         new System.Text.Json.
                                                                                             Transmogrify.
                                                                                             TransmogrifyJson()));
        }
        //todo check for errors in language pack pathing?

        [Theory]
        [InlineData("en", "Hello World!")]
        [InlineData("ru", "Привет Мир!")]
        public async Task ReturnCorrectTranslation_Json(string lang, string result)
        {
            var languageResolverMock = new Mock<ILanguageResolver>();
            languageResolverMock.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);

            var translator1 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new System.Text.Json.Transmogrify.TransmogrifyJson());
            var translator2 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new Microsoft.Extensions.DependencyInjection.Transmogrify.Newtonsoft.
                                                 TransmogrifyJson());
            var translation1 = await translator1.GetTranslation("main", "Hello");
            var translation2 = await translator2.GetTranslation("main", "Hello");
            Assert.Equal(result, translation1);
            Assert.Equal(result, translation2);
        }

        [Theory]
        [InlineData("en", "Hello World!", "Sec")]
        [InlineData("ru", "Привет Мир!", "Сек")]
        public async Task BothFilesAreLoadedInMemory(string pack, string expected1, string expected2)
        {
            var languageResolverMock1 = new Mock<ILanguageResolver>();
            languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(pack);

            var translator = new Translator(Config, new[] {languageResolverMock1.Object},
                                            new System.Text.Json.Transmogrify.TransmogrifyJson());

            var translation1 = await translator.GetTranslation("main", "Hello");
            var translation2 = await translator.GetTranslation("second", "Sec");
            Assert.Equal(expected1, translation1);
            Assert.Equal(expected2, translation2);
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

            var translator = new Translator(Config, new[] {languageResolverMock1.Object, languageResolverMock2.Object},
                                            new System.Text.Json.Transmogrify.TransmogrifyJson());

            var translation = await translator.GetTranslation("main", "Hello");
            Assert.Equal(result, translation);
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

            var translator = new Translator(Config, new[] {languageResolverMock1.Object, languageResolverMock2.Object},
                                            new System.Text.Json.Transmogrify.TransmogrifyJson());

            var translation = await translator.GetTranslation("main", "Hello");
            Assert.Equal(result, translation);
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