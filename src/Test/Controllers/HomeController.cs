using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Test.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger logger;
        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            using (logger.BeginScope("{event_type}{service}", "Signature.Success", "testService"))
            {
                logger.LogError("Logging with scope => {test}", "scope");
            }

            logger.LogError("Took place an error with event type: {event_type} with data: {test}", "ERROR", "test");
            return View();
        }
    }
}
