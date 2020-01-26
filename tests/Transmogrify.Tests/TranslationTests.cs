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

        private const string c_file = "main";
        private const string c_key = "Hello";
        private const string c_englishPack = "en";
        private const string c_russianPack = "ru";
        private const string c_englishResult = "Hello World!";
        private const string c_russianResult = "Привет Мир!";

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
        [InlineData(c_englishPack, c_englishResult)]
        [InlineData(c_russianPack, c_russianResult)]
        public async Task ReturnCorrectTranslation_Json(string lang, string result)
        {
            var languageResolverMock = new Mock<ILanguageResolver>();
            languageResolverMock.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);

            var translator1 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new System.Text.Json.Transmogrify.TransmogrifyJson());
            var translator2 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new Microsoft.Extensions.DependencyInjection.Transmogrify.Newtonsoft.
                                                 TransmogrifyJson());
            var translation1 = await translator1.GetTranslation(c_file, c_key);
            var translation2 = await translator2.GetTranslation(c_file, c_key);
            Assert.Equal(result, translation1);
            Assert.Equal(result, translation2);
        }

        [Theory]
        [InlineData(c_englishPack, c_russianPack, c_englishResult)]
        [InlineData(c_russianPack, c_englishPack, c_russianResult)]
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

            var translation = await translator.GetTranslation(c_file, c_key);
            Assert.Equal(result, translation);
        }

        [Theory]
        [InlineData("Albanian", c_russianPack, c_russianResult)]
        [InlineData("Albanian", c_englishPack, c_englishResult)]
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

            var translation = await translator.GetTranslation(c_file, c_key);
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
        //     var translation = await translator.GetTranslation(c_file, "Nest:Bob");
        //     Assert.Equal(expected, translation);
        // }
    }
}