using Microsoft.EntityFrameworkCore;
using MovilidadSostenible_YAMAHA.DBContext;
using System;
using System.Threading.Tasks;

namespace MovilidadSostenible_YAMAHA.Data
{
    public class ValidateConnectionBD
    {
        private readonly ApplicationDbContext _context;

        public ValidateConnectionBD(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool IsValid, string ErrorMessage)> ValidateConnectionAsync()
        {
            try
            {
                // Usamos CanConnectAsync para verificar la conexión
                var canConnect = await _context.Database.CanConnectAsync();
                return canConnect ? (true, string.Empty) : (false, "No se pudo conectar a la base de datos.");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}
