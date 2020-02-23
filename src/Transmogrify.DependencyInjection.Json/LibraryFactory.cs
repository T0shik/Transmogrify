using System.Text.Json;

namespace Transmogrify.DependencyInjection.Json
{
    public class LibraryFactory : LibraryFactoryBase
    {
        public LibraryFactory(TransmogrifyConfig transmogrifyConfig)
            : base(transmogrifyConfig) { }

        protected override T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}