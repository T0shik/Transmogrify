using System.Collections.Generic;
using System.Linq;
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
                        ["unique"] = "Unique"
                    },
                    ["second"] = new Dictionary<string, string>
                    {
                        ["Sec"] = "Sec",
                    },
                    ["unique"] =new Dictionary<string, string>
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
                    ["second"] = new Dictionary<string, string>
                    {
                        ["Sec"] = "Сек"
                    }
                }
            };
    }
}