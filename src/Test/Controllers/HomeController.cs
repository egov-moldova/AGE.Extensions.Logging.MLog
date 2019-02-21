using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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
            using (logger.BeginScope("{EventType}{Service}{EventTime}", "Signature.Success", "testService",DateTime.UtcNow))
            {
                logger.LogError("Logging with scope => {Test}", "scope");
            }

            logger.LogError("Took place an error with event type: {EventType} with data: {test}", "ERROR", "test");
            return View();
        }
    }
}
