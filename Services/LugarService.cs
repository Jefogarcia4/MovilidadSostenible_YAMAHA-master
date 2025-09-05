using Dapper;
using MovilidadSostenible_YAMAHA.Data;
using MovilidadSostenible_YAMAHA.Models;
using MySql.Data.MySqlClient;

namespace MovilidadSostenible_YAMAHA.Services
{
    public class LugarService
    {
        private readonly AmazonSecret _amazonSecret;

        public LugarService(AmazonSecret amazonSecret)
        {
            _amazonSecret = amazonSecret;
        }

        public async Task<List<LugarViewModel>> ObtenerLugaresAsync()
        {
            try
            {
                var secret = await _amazonSecret.GetSecretAsync();
                string connectionString = $"Server={secret.servername};Database={secret.database};Uid={secret.username};Pwd={secret.password};";

                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    var query = @"
                        SELECT id, nombre, direccion, latitud, longitud 
                        FROM lugares 
                        ORDER BY nombre";

                    var lugares = await connection.QueryAsync<LugarViewModel>(query);
                    return lugares.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener lugares: {ex.Message}");
            }
        }
    }
}