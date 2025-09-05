namespace MovilidadSostenible_YAMAHA.Models
{
    public class ResultadoViewModel
    {
        public int EncuestaId { get; set; }
        public DateTime Fecha { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public string RangoEdad { get; set; }
        public string TipoContrato { get; set; }
        public string Cargo { get; set; }
        public string AreaTrabajo { get; set; }
        public string Departamento { get; set; }
        public string Ciudad { get; set; }
        public string NombreBarrio { get; set; }
        public string Direccion { get; set; }
        public int Estrato { get; set; }
        public string SitioTrabajo { get; set; }
        public string DireccionSitioTrabajo { get; set; }
        public float DistanciaSedeResidencia { get; set; }
        public int DiasPromedioTrabajados { get; set; }
        public string ModoTransporteIda { get; set; }
        public string MedioTransporteHabitualIda { get; set; }
        public string TipoCombustibleIda { get; set; }
        public string ModeloVehiculoIda { get; set; }
        public int CilindrajeIda { get; set; }
        public bool ComparteVehiculoIda { get; set; }
        public string NombrePropietarioVehiculoIda { get; set; }
        public float EmisionKgCO2_Dia_Ida { get; set; }
        public float EmisionKgCO2_Dia_Regreso { get; set; }
        public float EmisionKgCO2_Semana { get; set; }
        public float EmisionKgCO2_Anual { get; set; }
    }

}
