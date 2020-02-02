using System.Collections.Generic;

namespace Transmogrify {
    public interface ILibraryFactory
    {
        Dictionary<string, Dictionary<string, Dictionary<string, string>>> GetOrLoad();
    }
}