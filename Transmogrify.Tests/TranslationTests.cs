using System.Threading.Tasks;
using Moq;
using Transmogrify;
using Xunit;

namespace Transmogrify.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ConfigurationLanguagePacksNotNullWhenCreated()
        {
            var configuration = new Config();
            Assert.NotNull(configuration.LanguagePacks);
        }
        
        [Theory]
        [InlineData("English", "Hello World!")]
        [InlineData("Russian", "Привет Мир!")]
        public async Task ReturnCorrectTranslation_Json(string lang, string result)
        {
            var config = new Config
            {
                LanguagePacks =
                {
                    ["English"] = "./Samples/english.json",
                    ["Russian"] = "./Samples/russian.json"
                },
            };
            
            var languageResolverMock = new Mock<ILanguageResolver>();
            languageResolverMock.Setup(x => x.GetLanguageCode()).ReturnsAsync(lang);

            var translator1 = new Translator(config, languageResolverMock.Object, new System.Text.Json.Transmogrify.TransmogrifyJson());
            var translator2 = new Translator(config, languageResolverMock.Object, new Newtonsoft.Json.Transmogrify.TransmogrifyJson());
            var translation1 = await translator1.GetTranslation("Hello");
            var translation2 = await translator2.GetTranslation("Hello");
            Assert.Equal(result, translation1);
            Assert.Equal(result, translation2);
        }
    }
}