using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Transmogrify;

namespace Sample.LanguageResolvers
{
    public class QueryLanguageResolver : ILanguageResolver
    {
        private readonly HttpContext _httpContext;

        public QueryLanguageResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Task<string> GetLanguageCode()
        {
            return Task.FromResult(_httpContext.Request.Query.TryGetValue("lang", out var lang)
                                       ? lang.FirstOrDefault()
                                       : "");
        }
    }
}