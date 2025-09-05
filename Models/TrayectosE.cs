using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovilidadSostenible_YAMAHA.Models
{
    [Table("trayectos")]
    public class TrayectosE
    {
        [Key]
        public int id_trayecto { get; set; }
        public int id_encuesta { get; set; }
        public string modo_transporte { get; set; }
        public TimeSpan tiempo_desplazamiento { get; set; }
        public string medio_transporte { get; set; }
        public string tipo_combustible { get; set; }
        public string modelo_vehiculo { get; set; }
        public string cilindraje { get; set; }
        public bool comparte_vehiculo { get; set; }
        public int? cantidad_personas { get; set; }
    }
}
