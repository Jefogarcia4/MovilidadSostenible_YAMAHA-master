using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovilidadSostenible_YAMAHA.Models
{
    [Table("transportepublico")]
    public class TransportePublicoE
    {
        [Key]
        public int id_transporte_publico { get; set; }
        public int id_encuesta { get; set; }
        public string medio_transporte { get; set; }
        public string punto_partida { get; set; }
        public string punto_llegada { get; set; }
        public decimal costo_semanal { get; set; }
    }
}
