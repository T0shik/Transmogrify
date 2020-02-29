namespace Transmogrify
{
    public interface IDictionary
    {
        string Read(string page, string phrase);
        string Read(string page, string phrase, params string[] parameters);
    }
}