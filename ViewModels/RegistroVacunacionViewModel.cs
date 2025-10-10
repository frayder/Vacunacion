using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class RegistroVacunacionViewModel
    {
        public int TotalRegistros { get; set; }
        public int EsquemasCompletos { get; set; }
        public int EsquemasIncompletos { get; set; }
        public int Pendientes { get; set; }
        public List<RegistroVacunacionItemViewModel> Registros { get; set; } = new();
    }

    public class RegistroVacunacionItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Consecutivo")]
        public string Consecutivo { get; set; } = string.Empty;

        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.Date)]
        public DateTime? FechaRegistro { get; set; }

        [Display(Name = "Primer Nombre")]
        [Required(ErrorMessage = "El primer nombre del paciente es obligatorio")]
        public string PrimerNombre { get; set; } = string.Empty;

        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;

        [Display(Name = "Primer Apellido")]
        [Required(ErrorMessage = "El primer apellido del paciente es obligatorio")]
        public string PrimerApellido { get; set; } = string.Empty;

        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Display(Name = "Tipo de Documento")]
        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Display(Name = "Número de Documento")]
        [Required(ErrorMessage = "El número de documento es obligatorio")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Aseguradora")]
        public int? AseguradoraId { get; set; }

        [Display(Name = "Régimen de Afiliación")]
        public int? RegimenAfiliacionId { get; set; }

        [Display(Name = "Pertenencia Étnica")]
        public int? PertenenciaEtnicaId { get; set; }

        [Display(Name = "Centro de Atención")]
        public int? CentroAtencionId { get; set; }

        [Display(Name = "Condición Usuario/a")]
        public int? CondicionUsuariaId { get; set; }

        [Display(Name = "Tipo de Carnet")]
        public int? TipoCarnetId { get; set; }

        [Display(Name = "Vacuna")]
        public string Vacuna { get; set; } = string.Empty;    

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;

        [Display(Name = "Notas Finales")]
        public string NotasFinales { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;

        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.Date)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Modificación")]
        [DataType(DataType.Date)]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Usuario Creador")]
        public int? UsuarioCreadorId { get; set; }

        [Display(Name = "Nombre Completo")]
        public string? NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Usuario Modificador")]
        public int? UsuarioModificadorId { get; set; }

        [Display(Name = "Fecha de Atención")]
        [DataType(DataType.Date)]
        public DateTime? FechaAtencion { get; set; }

        [Display(Name = "Esquema Completo")]
        public bool EsquemaCompleto { get; set; }

        [Display(Name = "Sexo")]
        public string? Sexo { get; set; }

        [Display(Name = "Orientación Sexual")]
        public string? OrientacionSexual { get; set; }

        [Display(Name = "Edad Gestacional")]
        public int? EdadGestacional { get; set; }

        [Display(Name = "Peso Infante")]
        public decimal? PesoInfante { get; set; }

        [Display(Name = "País de Nacimiento")]
        public string? PaisNacimiento { get; set; }

        [Display(Name = "Lugar de Parto")]
        public string? LugardeParto { get; set; }

        [Display(Name = "Estatus Migratorio")]
        public string? EstatusMigratorio { get; set; }

        [Display(Name = "Desplazado")]
        public bool Desplazado { get; set; }

        [Display(Name = "Discapacitado")]
        public bool Discapacitado { get; set; }

        [Display(Name = "Fallecido")]
        public bool Fallecido { get; set; }

        [Display(Name = "Víctima de Conflicto Armado")]
        public bool VictimaConflictoArmado { get; set; }

        [Display(Name = "Estudia")]
        public bool Estudia { get; set; }

        [Display(Name = "País de Residencia")]
        public string? PaisResidencia { get; set; }

        [Display(Name = "Departamento de Residencia")]
        public string? DepartamentoResidencia { get; set; }

        [Display(Name = "Municipio de Residencia")]
        public string? MunicipioResidencia { get; set; }

        [Display(Name = "Comuna/Localidad")]
        public string? ComunaLocalidad { get; set; }

        [Display(Name = "Área")]
        public string? Area { get; set; }

        [Display(Name = "Celular")]
        public string? Celular { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Autoriza Llamadas")]
        public bool AutorizaLlamadas { get; set; }

        [Display(Name = "Autoriza Envío Correo")]
        public bool AutorizaEnvioCorreo { get; set; }

        [Display(Name = "Relación")]
        public string? Relacion { get; set; }

        [Display(Name = "Antecedentes")]
        public string? ArrayAntecedentes { get; set; }

        [Display(Name = "Enfermedad/Contraindicación Vacuna")]
        public bool? EnfermedadContraindicacionVacuna { get; set; }

        [Display(Name = "Reacción Biológica")]
        public bool? ReaccionBiologico { get; set; }

        [Display(Name = "Madre/Cuidador")]
        public bool? MadreCuidador { get; set; }

        [Display(Name = "Tipo de Identificación Cuidador")]
        public string? TipoIdentificacionCuidador { get; set; }

        [Display(Name = "Número de Documento Cuidador")]
        public string? NumeroDocumentoCuidador { get; set; }

        [Display(Name = "Primer Nombre Cuidador")]
        public string? PrimerNombreCuidador { get; set; }

        [Display(Name = "Segundo Nombre Cuidador")]
        public string? SegundoNombreCuidador { get; set; }

        [Display(Name = "Primer Apellido Cuidador")]
        public string? PrimerApellidoCuidador { get; set; }

        [Display(Name = "Segundo Apellido Cuidador")]
        public string? SegundoApellidoCuidador { get; set; }
        
        [Display(Name = "Email Cuidador")]
        public string? EmailCuidador { get; set; }

        [Display(Name = "Teléfono Cuidador")]
        public string? TelefonoCuidador { get; set; }

        [Display(Name = "Celular Cuidador")]
        public string? CelularCuidador { get; set; }

        [Display(Name = "Régimen de Afiliación Cuidador")]
        public int? RegimenAfiliacionCuidador { get; set; }

        [Display(Name = "Pertenencia Étnica Cuidador")]
        public int? PertenenciaEtnicaIdCuidador { get; set; }

        [Display(Name = "Estado Desplazado Cuidador")]
        public bool? EstadoDesplazadoCuidador { get; set; }

        [Display(Name = "Parentesco Cuidador")]
        public string? ParentescoCuidador { get; set; }

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Display(Name = "Responsable")]
        public string? Responsable { get; set; }

        [Display(Name = "Ingreso PAIWEB")]
        public bool? IngresoPAIWEB { get; set; }

        [Display(Name = "Centro de Salud Responsable")]
        public string? CentroSaludResponsable { get; set; }

        [Display(Name = "Motivo de No Ingreso PAIWEB")]
        public string? MotivoNoIngresoPAIWEB { get; set; }
    }
}