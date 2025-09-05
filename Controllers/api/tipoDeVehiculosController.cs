using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.Models;
using MySql.Data.MySqlClient;

namespace MovilidadSostenible_YAMAHA.Controllers.api
{
    public class tipoDeVehiculosController : ControllerBase
    {
        private readonly AmazonSecret _amazonSecret;
        private readonly ValidateConnectionBD _connectionService;
        public tipoDeVehiculosController(IConfiguration configuration, AmazonSecret amazonSecret, ValidateConnectionBD connectionService)
        {
            _amazonSecret = amazonSecret;
            _connectionService = connectionService;
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerTiposDeVehiculos()
        {
            var (isConnectionValid, errorMessage) = await _connectionService.ValidateConnectionAsync();

            if (!isConnectionValid)
            {
                return StatusCode(500, new { success = false, message = errorMessage ?? "La conexión a la base de datos no es válida." });
            }
            var secret = await _amazonSecret.GetSecretAsync();

            if (secret == null || string.IsNullOrEmpty(secret.servername) || string.IsNullOrEmpty(secret.database))
            {
                return StatusCode(500, new { success = false, message = "No se pudieron obtener los detalles de la base de datos desde Secrets Manager." });
            }

            string connectionString = $"Server={secret.servername};Database={secret.database};Uid={secret.username};Pwd={secret.password};";

            List<TipoDeVehiculoE> listaDeVehiculos = new List<TipoDeVehiculoE>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT tipovehiculoId, vehiculo, tipocombustible FROM tipodevehiculos";
                MySqlCommand cmd = new MySqlCommand(query, connection);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listaDeVehiculos.Add(new TipoDeVehiculoE
                        {
                            tipovehiculoId = reader.GetInt32("tipovehiculoId"),
                            vehiculo = !reader.IsDBNull(reader.GetOrdinal("vehiculo")) ? reader.GetString("vehiculo") : string.Empty,
                            tipocombustible = !reader.IsDBNull(reader.GetOrdinal("tipocombustible")) ? reader.GetString("tipocombustible") : string.Empty
                        });
                    }
                }
            }

            return new JsonResult(listaDeVehiculos);
        }
    }
}
