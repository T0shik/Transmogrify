using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Transmogrify.Tests
{
    public class UnitTest1
    {
        private static TransmogrifyConfig Config =>
            new TransmogrifyConfig
            {
                LanguagePacks =
                {
                    ["English"] = "./Samples/english.json",
                    ["Russian"] = "./Samples/russian.json",
                },
            };

        private const string c_key = "Hello";
        private const string c_englishResult = "Hello World!";
        private const string c_russianResult = "Привет Мир!";

        [Fact]
        public void ConfigurationLanguagePacksNotNullWhenCreated()
        {
            var configuration = new TransmogrifyConfig();
            Assert.NotNull(configuration.LanguagePacks);
        }

        [Theory]
        [InlineData("English", c_englishResult)]
        [InlineData("Russian", c_russianResult)]
        public async Task ReturnCorrectTranslation_Json(string lang, string result)
        {
            var languageResolverMock = new Mock<ILanguageResolver>();
            languageResolverMock.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);

            var translator1 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new System.Text.Json.Transmogrify.TransmogrifyJson());
            var translator2 = new Translator(Config, new[] {languageResolverMock.Object},
                                             new Microsoft.Extensions.DependencyInjection.Transmogrify.Newtonsoft.TransmogrifyJson());
            var translation1 = await translator1.GetTranslation(c_key);
            var translation2 = await translator2.GetTranslation(c_key);
            Assert.Equal(result, translation1);
            Assert.Equal(result, translation2);
        }

        [Theory]
        [InlineData("English", "Russian", c_englishResult)]
        [InlineData("Russian", "English", c_russianResult)]
        public async Task LanguageResolversUsedInOrder(string first, string second, string result)
        {
            var languageResolverMock1 = new Mock<ILanguageResolver>();
            var languageResolverMock2 = new Mock<ILanguageResolver>();
            languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(first);
            languageResolverMock2.Setup(x => x.GetLanguageCode()).ReturnsAsync(second);
            
            var translator = new Translator(Config, new[] {languageResolverMock1.Object, languageResolverMock2.Object},
                                             new System.Text.Json.Transmogrify.TransmogrifyJson());

            var translation = await translator.GetTranslation(c_key);
            Assert.Equal(result, translation);
        }
        
        [Theory]
        [InlineData("Albanian", "Russian", c_russianResult)]
        [InlineData("Albanian", "English", c_englishResult)]
        public async Task SkipsMissingLanguageResolvers(string first, string second, string result)
        {
            var languageResolverMock1 = new Mock<ILanguageResolver>();
            var languageResolverMock2 = new Mock<ILanguageResolver>();
            languageResolverMock1.Setup(x => x.GetLanguageCode()).ReturnsAsync(first);
            languageResolverMock2.Setup(x => x.GetLanguageCode()).ReturnsAsync(second);
            
            var translator = new Translator(Config, new[] {languageResolverMock1.Object, languageResolverMock2.Object},
                                            new System.Text.Json.Transmogrify.TransmogrifyJson());

            var translation = await translator.GetTranslation(c_key);
            Assert.Equal(result, translation);
        }
    }
}