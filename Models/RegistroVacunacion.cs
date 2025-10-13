using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Models
{
    public class RegistrosVacunacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El consecutivo es obligatorio")]
        [StringLength(20, ErrorMessage = "El consecutivo no puede tener más de 20 caracteres")]
        [Display(Name = "Consecutivo")]
        public string Consecutivo { get; set; } = string.Empty;

        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.Date)]
        public DateTime? FechaRegistro { get; set; }

        // DATOS DEL PACIENTE
        [Required(ErrorMessage = "Primer Nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener más de 255 caracteres")]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = string.Empty;

        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Primer Apellido es obligatorio")]
        [StringLength(255, ErrorMessage = "El apellido no puede tener más de 255 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de documento es obligatorio")]
        [StringLength(10, ErrorMessage = "El tipo de documento no puede tener más de 10 caracteres")]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de documento no puede tener más de 20 caracteres")]
        [Display(Name = "Número de Documento")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, ErrorMessage = "El género no puede tener más de 10 caracteres")]
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(500, ErrorMessage = "La dirección no puede tener más de 500 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        // DATOS DE AFILIACIÓN
        [Display(Name = "Aseguradora")]
        [ForeignKey("Aseguradora")]
        public int? AseguradoraId { get; set; }
        public virtual Aseguradora? Aseguradora { get; set; }

        [Display(Name = "Régimen de Afiliación")]
        [ForeignKey("RegimenAfiliacion")]
        public int? RegimenAfiliacionId { get; set; }
        public virtual RegimenAfiliacion? RegimenAfiliacion { get; set; }

        [Display(Name = "Pertenencia Étnica")]
        [ForeignKey("PertenenciaEtnica")]
        public int? PertenenciaEtnicaId { get; set; }
        public virtual PertenenciaEtnica? PertenenciaEtnica { get; set; }

        // DATOS DE ATENCIÓN
        [Display(Name = "Centro de Atención")]
        [ForeignKey("CentroAtencion")]
        public int? CentroAtencionId { get; set; }
        public virtual CentroAtencion? CentroAtencion { get; set; }

        [Display(Name = "Condición Usuario/a")]
        [ForeignKey("CondicionUsuaria")]
        public int? CondicionUsuariaId { get; set; }
        public virtual CondicionUsuaria? CondicionUsuaria { get; set; }

        [Display(Name = "Tipo de Carnet")]
        [ForeignKey("TipoCarnet")]
        public int? TipoCarnetId { get; set; }
        public virtual TipoCarnet? TipoCarnet { get; set; }

        // DATOS DE LA VACUNA
        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la vacuna no puede tener más de 100 caracteres")]
        [Display(Name = "Vacuna")]
        public string Vacuna { get; set; } = string.Empty;  

        // OBSERVACIONES Y NOTAS
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [StringLength(1000, ErrorMessage = "Las notas finales no pueden tener más de 1000 caracteres")]
        [Display(Name = "Notas Finales")]
        public string? NotasFinales { get; set; }

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
        [Precision(10, 3)]
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
        public virtual ICollection<AntecedenteMedico> AntecedentesMedicos { get; set; } = new List<AntecedenteMedico>();


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

        [Display(Name =  "Fecha Aplicacion")] 
        public DateTime? FechaAplicacion { get; set; }

        [Display(Name = "Responsable")]
        public string? Responsable { get; set; }

        [Display(Name = "Ingreso PAIWEB")]
        public bool? IngresoPAIWEB { get; set; }

        [Display(Name = "Centro de Salud Responsable")]
        public string? CentroSaludResponsable { get; set; }

        // CAMPOS DE AUDITORÍA
        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Usuario Creador")]
        [ForeignKey("UsuarioCreador")]
        public int? UsuarioCreadorId { get; set; }
        public virtual User? UsuarioCreador { get; set; }

        [Display(Name = "Usuario Modificador")]
        [ForeignKey("UsuarioModificador")]
        public int? UsuarioModificadorId { get; set; }
        public virtual User? UsuarioModificador { get; set; }

        // PROPIEDADES CALCULADAS
        // [NotMapped]
        // public int EdadAlMomento 
        // {
        //     get
        //     {
        //         var fechaReferencia = FechaAplicacion != default ? FechaAplicacion : DateTime.Now;
        //         var edad = fechaReferencia.Year - FechaNacimiento.Year;
        //         if (FechaNacimiento.Date > fechaReferencia.AddYears(-edad).Date)
        //             edad--;
        //         return edad;
        //     }
        // }
 
        public string NombreCompleto { get; set; } = string.Empty;

        [NotMapped]
        public string DocumentoCompleto => $"{TipoDocumento} {NumeroDocumento}";

        [NotMapped]
        public string VacunaCompleta => $"{Vacuna} - {Dosis}";

        [Display(Name = "Motivo de No Ingreso PAIWEB")]
        public string? MotivoNoIngresoPAIWEB { get; set; }

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
    }
}