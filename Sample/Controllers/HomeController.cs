using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Transmogrify;

namespace Sample.Controllers
{
    public class HomeController : Controller
    {
        private ITranslator _translator;

        public HomeController(ITranslator translator)
        {
            _translator = translator;
        }

        public async Task<string> Index()
        {
            return await _translator.GetTranslation("Hello");
        }
    }
}