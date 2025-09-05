using Microsoft.AspNetCore.Mvc;

namespace MovilidadSostenible_YAMAHA.Controllers
{
    [Route("Encuesta")]
    public class TomaDatosController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
