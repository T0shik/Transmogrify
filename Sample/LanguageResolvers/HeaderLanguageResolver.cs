using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Transmogrify;

namespace Sample.LanguageResolvers {
    public class HeaderLanguageResolver : ILanguageResolver
    {
        private readonly HttpContext _httpContext;

        public HeaderLanguageResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public Task<string> GetLanguageCode()
        {
            var header = _httpContext.Request.Headers["Accept-Language"];
            return Task.FromResult(header.FirstOrDefault());
        }
    }
}