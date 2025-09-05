using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class EncuestaViewModel
{
    [Required]
    public DateTime fecha { get; set; }
    [Required]
    public string? nombreCompleto { get; set; }
    [Required]
    public string? tipoIdentificacion { get; set; }
    [Required]
    public string? numeroIdentificacion { get; set; }
    [Required]
    public string? fechaNacimiento { get; set; }
    [Required]
    public string? genero { get; set; }
    [Required]
    public string? rangoEdad { get; set; }
    [Required]
    public string? tipoContrato { get; set; }
    [Required]
    public string? cargo { get; set; }
    [Required]
    public string? areaTrabajo { get; set; }
    [Required]
    public string? departamento { get; set; }
    [Required]
    public string? ciudad { get; set; }
    [Required]
    public string? nombreBarrio { get; set; }
    [Required]
    public string? direccion { get; set; }
    [Required]
    public string? estrato { get; set; }
    [Required]
    public string? sitioTrabajo { get; set; }
    public string? direccionSitioTrabajo { get; set; }
    [Required]
    public string? diasPromedioTrabajados { get; set; }
    [Required]
    public string? distanciaSedeResidencia { get; set; }
    [Required]
    public string? modoTransporteida { get; set; }
    [Required]
    public string? tiempoDesplazamientoida { get; set; }
    [Required]
    public int medioTransporteHabitualidaid { get; set; }
    [Required]
    public string? tipoCombustibleida { get; set; }
    public string? modeloVehiculoida { get; set; }
    public string? cilindrajeida { get; set; }
    public string? comparteVehiculoida { get; set; }
    public string? siComparteVehiculoida { get; set; }
    public string? nombrePropietariovehiculoida { get; set; }
    public int mediotransporteidatrayecto1id { get; set; }
    public string? puntopartidaidatrayecto1 { get; set; }
    public string? puntodestinoidatrayecto1 { get; set; }
    public string? distanciaidatrayecto1 { get; set; }
    public int mediotransporteidatrayecto2id { get; set; }
    public string? puntopartidaidatrayecto2 { get; set; }
    public string? puntodestinoidatrayecto2 { get; set; }
    public string? distanciaidatrayecto2 { get; set; }
    public int mediotransporteidatrayecto3id { get; set; }
    public string? puntopartidaidatrayecto3 { get; set; }
    public string? puntodestinoidatrayecto3 { get; set; }
    public string? distanciaidatrayecto3 { get; set; }
    public string? costosemanalsistematransporteida { get; set; }
    [Required]
    public string? modoTransporteregreso { get; set; }
    [Required]
    public string? tiempoDesplazamientoregreso { get; set; }
    [Required]
    public int medioTransporteHabitualregresoid { get; set; }
    public string? tipoCombustibleregreso { get; set; }
    public string? modeloVehiculoregreso { get; set; }
    public string? cilindrajeregreso { get; set; }
    public string? comparteVehiculoregreso { get; set; }
    public string? siComparteVehiculoregreso { get; set; }
    public string? nombrePropietariovehiculoregreso { get; set; }
    public int mediotransporteregresotrayecto1id { get; set; }
    public string? puntopartidaregresotrayecto1 { get; set; }
    public string? puntodestinoregresotrayecto1 { get; set; }
    public string? distanciaregresotrayecto1 { get; set; }
    public int mediotransporteregresotrayecto2id { get; set; }
    public string? puntopartidaregresotrayecto2 { get; set; }
    public string? puntodestinoregresotrayecto2 { get; set; }
    public string? distanciaregresotrayecto2 { get; set; }
    public int mediotransporteregresotrayecto3id { get; set; }
    public string? puntopartidaregresotrayecto3 { get; set; }
    public string? puntodestinoregresotrayecto3 { get; set; }
    public string? distanciaregresotrayecto3 { get; set; }
    public string? costosemanalsistematransporteregreso { get; set; }

}

