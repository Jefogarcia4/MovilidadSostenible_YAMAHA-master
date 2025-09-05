using Microsoft.AspNetCore.Mvc;

namespace MovilidadSostenible_YAMAHA.Controllers
{
    public class PuntosAtencionController : Controller
    {
        [Route("Puntos-de-atencion")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
