using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.Services;

namespace MovilidadSostenible_YAMAHA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LugarController : ControllerBase
    {
        private readonly LugarService _lugarService;

        public LugarController(LugarService lugarService)
        {
            _lugarService = lugarService;
        }

        [HttpGet]
        public async Task<IActionResult> GetLugares()
        {
            try
            {
                var lugares = await _lugarService.ObtenerLugaresAsync();
                return Ok(lugares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}