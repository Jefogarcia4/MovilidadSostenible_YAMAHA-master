using System.ComponentModel.DataAnnotations;

namespace MovilidadSostenible_YAMAHA.Models
{
    public class LugarViewModel
    {
        public int Id { get; set; }
        
        [Required]
        public string Nombre { get; set; }
        
        [Required]
        public string Direccion { get; set; }
        
        public decimal? Latitud { get; set; }
        
        public decimal? Longitud { get; set; }
    }
}