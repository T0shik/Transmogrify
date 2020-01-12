namespace Transmogrify {
    public interface ITransmogrifyJson
    {
        T Deserialize<T>(string value);
    }
}