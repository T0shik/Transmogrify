using Transmogrify;

namespace System.Text.Json.Transmogrify
{
    public class TransmogrifyJson : ITransmogrifyJson
    {
        public T Deserialize<T>(string value)
        {
            return JsonSerializer.Deserialize<T>(value);
        }
    }
}