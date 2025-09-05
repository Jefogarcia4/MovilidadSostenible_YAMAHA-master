using Dapper;
using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.ProxyClient;
using MovilidadSostenible_YAMAHA.Models; // Agregar esta línea si no existe
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;

namespace MovilidadSostenible_YAMAHA.Controllers
{
    public class EncuestaController : Controller
    {
        private readonly ColombiaService _colombiaService;
        private readonly ValidateConnectionBD _connectionService;
        private readonly AmazonSecret _amazonSecret;

        public EncuestaController(ColombiaService colombiaService, IConfiguration configuration, AmazonSecret amazonSecret, ValidateConnectionBD connectionService)
        {
            _colombiaService = colombiaService;
            _amazonSecret = amazonSecret;
            _connectionService = connectionService;

        }

        [HttpPost]
        public async Task<IActionResult> GuardarEncuesta([FromBody] EncuestaViewModel encuesta)
        {
            var emptyProperties = GetEmptyRequiredProperties(encuesta);

          


            if (emptyProperties.Any())
            {
                return Json(new
                {
                    success = false,
                    message = $"Los siguientes campos no pueden estar vacíos: {string.Join(", ", emptyProperties)}",
                    errores = emptyProperties.ToDictionary(prop => prop, prop => "Este campo es obligatorio")
                });
            }

            EncuestaViewModel encuestaResult = reemplazodistancia(encuesta).Result;

            try
            {
                var jsonEncuesta = JsonSerializer.Serialize(encuesta);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                };

                Response.Cookies.Append("EncuestaCookie", jsonEncuesta, cookieOptions);
                ObtenerEncuesta();

                var insercionEncuestaResult = await InsertarEncuesta(encuestaResult);

                return insercionEncuestaResult;
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Hubo un problema al enviar la encuesta: " + ex.Message });
            }
        }

        private List<string> GetEmptyRequiredProperties(EncuestaViewModel encuesta)
        {
            var properties = encuesta.GetType().GetProperties();

            var requiredProperties = properties.Where(p => p.GetCustomAttribute<RequiredAttribute>() != null);

            var emptyProperties = new List<string>();

            foreach (var property in requiredProperties)
            {
                var value = property.GetValue(encuesta);

                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    emptyProperties.Add(property.Name); // Agregar el nombre de la propiedad vacía
                }
            }

            return emptyProperties; // Retorna la lista de propiedades vacías
        }


        private bool AreRequiredPropertiesEmpty(EncuestaViewModel encuesta)
        {
            var properties = encuesta.GetType().GetProperties();

            var requiredProperties = properties.Where(p => p.GetCustomAttribute<RequiredAttribute>() != null);

            // Iterar sobre las propiedades requeridas
            foreach (var property in requiredProperties)
            {
                // Obtener el valor de la propiedad
                var value = property.GetValue(encuesta);

                // Verificar si la propiedad es nula o una cadena vacía
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return true;  // Si alguna propiedad requerida está vacía o nula, retorna true
                }
            }
            return false;  // Si todas las propiedades requeridas están completas, retorna false
        }
        public IActionResult ObtenerEncuesta()
        {
            if (Request.Cookies.TryGetValue("EncuestaCookie", out var jsonEncuesta))
            {
                // Deserializar el JSON de vuelta al objeto Encuesta
                var encuesta = JsonSerializer.Deserialize<EncuestaViewModel>(jsonEncuesta);
                return Ok(encuesta);
            }

            return NotFound("No se encontró la encuesta en la cookie.");
        }

        public async Task<IActionResult> InsertarEncuesta(EncuestaViewModel encuesta)
        {
            var (isConnectionValid, errorMessage) = await _connectionService.ValidateConnectionAsync();

            if (!isConnectionValid)
            {
                return StatusCode(500, new { success = false, message = errorMessage ?? "La conexión a la base de datos no es válida." });
            }

            try
            {
                var secret = await _amazonSecret.GetSecretAsync();

                if (secret == null || string.IsNullOrEmpty(secret.servername) || string.IsNullOrEmpty(secret.database))
                {
                    return StatusCode(500, new { success = false, message = "No se pudieron obtener los detalles de la base de datos desde Secrets Manager." });
                }

                string connectionString = $"Server={secret.servername};Database={secret.database};Uid={secret.username};Pwd={secret.password};";

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var parameters = new DynamicParameters();
                    parameters.Add("p_fecha", encuesta.fecha);
                    parameters.Add("p_nombreCompleto", encuesta.nombreCompleto);
                    parameters.Add("p_tipoIdentificacion", encuesta.tipoIdentificacion);
                    parameters.Add("p_numeroIdentificacion", encuesta.numeroIdentificacion);
                    parameters.Add("p_fechaNacimiento", encuesta.fechaNacimiento);
                    parameters.Add("p_genero", encuesta.genero);
                    parameters.Add("p_rangoEdad", encuesta.rangoEdad);
                    parameters.Add("p_tipoContrato", encuesta.tipoContrato);
                    parameters.Add("p_cargo", encuesta.cargo);
                    parameters.Add("p_areaTrabajo", encuesta.areaTrabajo);
                    parameters.Add("p_departamento", encuesta.departamento);
                    parameters.Add("p_ciudad", encuesta.ciudad);
                    parameters.Add("p_nombreBarrio", encuesta.nombreBarrio);
                    parameters.Add("p_direccion", encuesta.direccion);
                    parameters.Add("p_estrato", encuesta.estrato);
                    parameters.Add("p_sitioTrabajo", encuesta.sitioTrabajo);
                    parameters.Add("p_direccionSitioTrabajo", encuesta.direccionSitioTrabajo);
                    parameters.Add("p_distanciaSedeResidencia", encuesta.distanciaSedeResidencia);
                    parameters.Add("p_diasPromedioTrabajados", encuesta.diasPromedioTrabajados);
                    parameters.Add("p_modoTransporteida", encuesta.modoTransporteida);
                    parameters.Add("p_tiempoDesplazamientoida", encuesta.tiempoDesplazamientoida);
                    parameters.Add("p_medioTransporteHabitualidaid", encuesta.medioTransporteHabitualidaid);
                    parameters.Add("p_tipoCombustibleida", encuesta.tipoCombustibleida);
                    parameters.Add("p_modeloVehiculoida", encuesta.modeloVehiculoida);
                    parameters.Add("p_cilindrajeida", encuesta.cilindrajeida);
                    parameters.Add("p_comparteVehiculoida", encuesta.comparteVehiculoida);
                    parameters.Add("p_siComparteVehiculoida", encuesta.siComparteVehiculoida);
                    parameters.Add("p_nombrePropietariovehiculoida", encuesta.nombrePropietariovehiculoida);
                    parameters.Add("p_mediotransporteidatrayecto1id", encuesta.mediotransporteidatrayecto1id);
                    parameters.Add("p_puntopartidaidatrayecto1", encuesta.puntopartidaidatrayecto1);
                    parameters.Add("p_puntodestinoidatrayecto1", encuesta.puntodestinoidatrayecto1);
                    parameters.Add("p_distanciaidatrayecto1", encuesta.distanciaidatrayecto1);
                    parameters.Add("p_mediotransporteidatrayecto2id", encuesta.mediotransporteidatrayecto2id);
                    parameters.Add("p_puntopartidaidatrayecto2", encuesta.puntopartidaidatrayecto2);
                    parameters.Add("p_puntodestinoidatrayecto2", encuesta.puntodestinoidatrayecto2);
                    parameters.Add("p_distanciaidatrayecto2", encuesta.distanciaidatrayecto2);
                    parameters.Add("p_mediotransporteidatrayecto3id", encuesta.mediotransporteidatrayecto3id);
                    parameters.Add("p_puntopartidaidatrayecto3", encuesta.puntopartidaidatrayecto3);
                    parameters.Add("p_puntodestinoidatrayecto3", encuesta.puntodestinoidatrayecto3);
                    parameters.Add("p_distanciaidatrayecto3", encuesta.distanciaidatrayecto3);
                    parameters.Add("p_costosemanalsistematransporteida", encuesta.costosemanalsistematransporteida);
                    parameters.Add("p_modoTransporteregreso", encuesta.modoTransporteregreso);
                    parameters.Add("p_tiempoDesplazamientoregreso", encuesta.tiempoDesplazamientoregreso);
                    parameters.Add("p_medioTransporteHabitualregresoid", encuesta.medioTransporteHabitualregresoid);
                    parameters.Add("p_tipoCombustibleregreso", encuesta.tipoCombustibleregreso);
                    parameters.Add("p_modeloVehiculoregreso", encuesta.modeloVehiculoregreso);
                    parameters.Add("p_cilindrajeregreso", encuesta.cilindrajeregreso);
                    parameters.Add("p_comparteVehiculoregreso", encuesta.comparteVehiculoregreso);
                    parameters.Add("p_siComparteVehiculoregreso", encuesta.siComparteVehiculoregreso);
                    parameters.Add("p_nombrePropietariovehiculoregreso", encuesta.nombrePropietariovehiculoregreso);
                    parameters.Add("p_mediotransporteregresotrayecto1id", encuesta.mediotransporteregresotrayecto1id);
                    parameters.Add("p_puntopartidaregresotrayecto1", encuesta.puntopartidaregresotrayecto1);
                    parameters.Add("p_puntodestinoregresotrayecto1", encuesta.puntodestinoregresotrayecto1);
                    parameters.Add("p_distanciaregresotrayecto1", encuesta.distanciaregresotrayecto1);
                    parameters.Add("p_mediotransporteregresotrayecto2id", encuesta.mediotransporteregresotrayecto2id);
                    parameters.Add("p_puntopartidaregresotrayecto2", encuesta.puntopartidaregresotrayecto2);
                    parameters.Add("p_puntodestinoregresotrayecto2", encuesta.puntodestinoidatrayecto2);
                    parameters.Add("p_distanciaregresotrayecto2", encuesta.distanciaregresotrayecto2);
                    parameters.Add("p_mediotransporteregresotrayecto3id", encuesta.mediotransporteregresotrayecto3id);
                    parameters.Add("p_puntopartidaregresotrayecto3", encuesta.puntopartidaregresotrayecto3);
                    parameters.Add("p_puntodestinoregresotrayecto3", encuesta.puntodestinoregresotrayecto3);
                    parameters.Add("p_distanciaregresotrayecto3", encuesta.distanciaregresotrayecto3);
                    parameters.Add("p_costosemanalsistematransporteregreso", encuesta.costosemanalsistematransporteregreso);

                    var result = await connection.ExecuteAsync("InsertarEncuesta", parameters, commandType: System.Data.CommandType.StoredProcedure);

                    if (result > 0)
                    {
                        return Json(new { success = true, message = "La encuesta se guardó correctamente." });
                    }
                    else
                    {
                        return StatusCode(500, new { success = false, message = "No se pudo guardar la encuesta." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Hubo un problema al guardar la encuesta: " + ex.Message });
            }
        }

        private async Task<EncuestaViewModel> reemplazodistancia(EncuestaViewModel encuesta)
        {
            string dissedresid = encuesta.distanciaSedeResidencia.Replace("<br>", "-");
            encuesta.distanciaSedeResidencia = dissedresid;

            string distanciaTrayecto1ida = encuesta.distanciaidatrayecto1.Replace("<br>", "-");
            encuesta.distanciaidatrayecto1 = distanciaTrayecto1ida.Replace(",",".");

            string distanciaTrayecto2ida = encuesta.distanciaidatrayecto2.Replace("<br>", "-");
            encuesta.distanciaidatrayecto2 = distanciaTrayecto2ida.Replace(",", ".");

            string distanciaTrayecto3ida = encuesta.distanciaidatrayecto3.Replace("<br>", "-");
            encuesta.distanciaidatrayecto3 = distanciaTrayecto3ida.Replace(",", "."); ;

            string distanciaTrayecto1regreso = encuesta.distanciaregresotrayecto1.Replace("<br>", "-");
            encuesta.distanciaregresotrayecto1 = distanciaTrayecto1regreso.Replace(",", ".");

            string distanciaTrayecto2regreso = encuesta.distanciaregresotrayecto2.Replace("<br>", "-");
            encuesta.distanciaregresotrayecto2 = distanciaTrayecto2regreso.Replace(",", "."); ;

            string distanciaTrayecto3regreso = encuesta.distanciaregresotrayecto3.Replace("<br>", "-");
            encuesta.distanciaregresotrayecto3 = distanciaTrayecto3regreso.Replace(",", ".");

            int medioTransporte = 0;

            if (encuesta.puntopartidaidatrayecto1 == "" && encuesta.puntodestinoidatrayecto1 == "")
            {
                encuesta.mediotransporteidatrayecto1id = medioTransporte;
            }

            if (encuesta.puntopartidaidatrayecto2 == "" && encuesta.puntodestinoidatrayecto2 == "")
            {
                encuesta.mediotransporteidatrayecto2id = medioTransporte;
            }

            if (encuesta.puntopartidaidatrayecto3 == "" && encuesta.puntodestinoidatrayecto3 == "")
            {
                encuesta.mediotransporteidatrayecto3id = medioTransporte;
            }

            if (encuesta.puntopartidaregresotrayecto1 == "" && encuesta.puntodestinoregresotrayecto1 == "")
            {
                encuesta.mediotransporteregresotrayecto1id = medioTransporte;
            }

            if (encuesta.puntopartidaregresotrayecto2 == "" && encuesta.puntodestinoregresotrayecto2 == "")
            {
                encuesta.mediotransporteregresotrayecto2id = medioTransporte;
            }

            if (encuesta.puntopartidaregresotrayecto3 == "" && encuesta.puntodestinoregresotrayecto3 == "")
            {
                encuesta.mediotransporteregresotrayecto3id = medioTransporte;
            }

            return encuesta;
        }

    }

}
