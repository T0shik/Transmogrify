using Transmogrify;

namespace Newtonsoft.Json.Transmogrify
{
    public class TransmogrifyJson : ITransmogrifyJson
    {
        public T Deserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}