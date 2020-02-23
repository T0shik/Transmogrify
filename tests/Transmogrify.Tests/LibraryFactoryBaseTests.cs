using System;
using Transmogrify.Exceptions;
using Xunit;

namespace Transmogrify.Tests
{
    public class LibraryFactoryBaseTests
    {
        public class BaseMock : LibraryFactoryBase
        {
            public BaseMock(TransmogrifyConfig transmogrifyConfig)
                : base(transmogrifyConfig) { }

            protected override T Deserialize<T>(string value)
            {
                throw new NotImplementedException();
            }
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ThrowsIfLanguagePathIsNullOrEmpty(string path)
        {
            BaseMock Action() => new BaseMock(new TransmogrifyConfig {LanguagePath = path});

            Assert.Throws<TransmogrifyInvalidLanguagePath>(Action);
        }
    }
}