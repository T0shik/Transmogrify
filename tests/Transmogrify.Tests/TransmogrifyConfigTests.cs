using System.Linq;
using System.Threading.Tasks;
using Moq;
using Transmogrify.Exceptions;
using Xunit;

namespace Transmogrify.Tests
{
    public class TransmogrifyConfigTests
    {
        [Fact]
        public void ThrowsWhenWrongResolverType()
        {
            var config = new TransmogrifyConfig();
            Assert.Throws<TransmogrifyInvalidLanguageResolverType>(() => config.AddResolver(typeof(string)));
        }
        
        public class MockResolver : ILanguageResolver
        {
            public Task<string> GetLanguageCode() {}
        }
        
        [Fact]
        public void AddsResolvers()
        {
            var config = new TransmogrifyConfig();
            config.AddResolver(typeof(MockResolver));

            var resolver = config.LanguageResolvers.Single();
            Assert.Equal("MockResolver", resolver.Name);
        }
    }
}