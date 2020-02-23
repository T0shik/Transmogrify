namespace Transmogrify
{
    public interface IReader
    {
        string Read(string page, string phrase);
        string Read(string page, string phrase, params string[] parameters);
    }
}