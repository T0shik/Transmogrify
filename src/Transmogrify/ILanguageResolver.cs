using System.Threading.Tasks;

namespace Transmogrify
{
    public interface ILanguageResolver
    {
        Task<string> GetLanguageCode();
    }
}