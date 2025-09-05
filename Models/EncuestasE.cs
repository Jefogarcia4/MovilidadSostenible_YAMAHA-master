using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovilidadSostenible_YAMAHA.Models
{
    [Table("encuestas")]
    public class EncuestasE
    {
        [Key]
        public int encuestaId { get; set; }
        public DateTime fecha { get; set; }
        public string nombreCompleto { get; set; }
        public string tipoIdentificacion { get; set; }
        public string numeroIdentificacion { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string genero { get; set; } // Enum: Masculino, Femenino, Otro
        public string rangoEdad { get; set; }
        public string tipoContrato { get; set; }
        public string cargo { get; set; }
        public string areaTrabajo { get; set; }
        public string departamento { get; set; }
        public string ciudad { get; set; }
        public string nombreBarrio { get; set; }
        public string direccion { get; set; }
        public int estrato { get; set; }
        public string sitioTrabajo { get; set; }
        public string direccionSitioTrabajo { get; set; }
        public string distanciaSedeResidencia { get; set; }
        public int diasPromedioTrabajados { get; set; }
    }
}
