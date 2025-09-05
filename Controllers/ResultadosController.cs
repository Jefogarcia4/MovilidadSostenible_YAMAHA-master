using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.Models;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using System.Data;

namespace MovilidadSostenible_YAMAHA.Controllers
{

    public class ResultadosController : Controller
    {
        public string connectionString;
        private readonly AmazonSecret _amazonSecret;
        private readonly ValidateConnectionBD _connectionService;
        public ResultadosController(IConfiguration configuration, AmazonSecret amazonSecret, ValidateConnectionBD connectionService)
        {
            _amazonSecret = amazonSecret;
            _connectionService = connectionService;
        }
        public async Task<IActionResult> Index()
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

            connectionString = $"Server={secret.servername};Database={secret.database};Uid={secret.username};Pwd={secret.password};";

            List<ResultadoViewModel> resultados = new List<ResultadoViewModel>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new MySqlCommand("ListarResultadoEncuestas", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(new ResultadoViewModel
                            {
                                EncuestaId = reader.GetInt32("encuestaId"),
                                Fecha = reader.GetDateTime("fecha"),
                                NombreCompleto = reader.GetString("nombreCompleto"),
                                TipoIdentificacion = reader.GetString("tipoIdentificacion"),
                                NumeroIdentificacion = reader.GetString("numeroIdentificacion"),
                                FechaNacimiento = reader.GetDateTime("fechaNacimiento"),
                                Genero = reader.GetString("genero"),
                                RangoEdad = reader.GetString("rangoEdad"),
                                TipoContrato = reader.GetString("tipoContrato"),
                                Cargo = reader.GetString("cargo"),
                                Ciudad = reader.GetString("ciudad"),
                                Direccion = reader.GetString("direccion"),
                                MedioTransporteHabitualIda = reader.GetString("mediotransportehabitualida"),
                                EmisionKgCO2_Dia_Ida = reader.GetFloat("EmisionKgCO2_Dia_Ida"),
                                EmisionKgCO2_Dia_Regreso= reader.GetFloat("EmisionKgCO2_Dia_Regreso"),
                                EmisionKgCO2_Semana = reader.GetFloat("EmisionKgCO2_Semana"),
                                EmisionKgCO2_Anual = reader.GetFloat("EmisionKgCO2_Anual")
                            });
                        }
                    }
                }
            }

            return View(resultados);
        }


        [HttpGet("exportar")]
        public async  Task<IActionResult> Exportar()
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

            connectionString = $"Server={secret.servername};Database={secret.database};Uid={secret.username};Pwd={secret.password};";

            string query = "CALL ListarResultadoEncuestas()";

            DataTable dt = new DataTable();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "ResultadosEncuestas");
                string fileName = "ResultadosEncuestas.xlsx";
                string filePath = Path.Combine(Path.GetTempPath(), fileName);
                wb.SaveAs(filePath);

                // Descargar el archivo al usuario
                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            
        }
    }
}
