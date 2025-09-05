using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.ProxyClient;

namespace MovilidadSostenible_YAMAHA.Controllers.api
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeographicalController : ControllerBase
    {
        private readonly ColombiaService _geographicalService;

        public GeographicalController(ColombiaService geographicalService)
        {
            _geographicalService = geographicalService;
        }

        [HttpGet("departamentos")]
        public async Task<IActionResult> GetListDepartamentos()
        {
            var departments = await _geographicalService.GetListDepartamentosAsync();
            return Ok(departments);
        }

        [HttpGet("ciudades/{departamentoId}")]
        public async Task<IActionResult> GetListCiudades(int departamentoId)
        {

            var cities = await _geographicalService.GetListCiudadesAsync(departamentoId);
            return Ok(cities);
        }
    }
}
