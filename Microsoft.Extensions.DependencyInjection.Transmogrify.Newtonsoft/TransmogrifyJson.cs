using Newtonsoft.Json;
using Transmogrify;

namespace Microsoft.Extensions.DependencyInjection.Transmogrify.Newtonsoft
{
    public class TransmogrifyJson : ITransmogrifyJson
    {
        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}