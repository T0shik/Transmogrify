using Newtonsoft.Json;

namespace Transmogrify.DependencyInjection.Newtonsoft
{
    public class LibraryFactory : LibraryFactoryBase
    {
        public LibraryFactory(TransmogrifyConfig transmogrifyConfig)
            : base(transmogrifyConfig) { }

        protected override T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}