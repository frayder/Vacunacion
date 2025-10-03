using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class RegistroVacunacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El consecutivo es obligatorio")]
        [StringLength(20, ErrorMessage = "El consecutivo no puede tener más de 20 caracteres")]
        [Display(Name = "Consecutivo")]
        public string Consecutivo { get; set; } = string.Empty;

        // DATOS DEL PACIENTE
        [Required(ErrorMessage = "El nombre del paciente es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener más de 255 caracteres")]
        [Display(Name = "Nombres y Apellidos")]
        public string NombresApellidos { get; set; } = string.Empty;

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

        [Required(ErrorMessage = "El número de dosis es obligatorio")]
        [StringLength(20, ErrorMessage = "El número de dosis no puede tener más de 20 caracteres")]
        [Display(Name = "Número de Dosis")]
        public string NumeroDosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de aplicación es obligatoria")]
        [Display(Name = "Fecha de Aplicación")]
        [DataType(DataType.Date)]
        public DateTime FechaAplicacion { get; set; }

        [StringLength(50, ErrorMessage = "El lote no puede tener más de 50 caracteres")]
        [Display(Name = "Lote")]
        public string? Lote { get; set; }

        [StringLength(100, ErrorMessage = "El laboratorio no puede tener más de 100 caracteres")]
        [Display(Name = "Laboratorio")]
        public string? Laboratorio { get; set; }

        [StringLength(50, ErrorMessage = "La vía de administración no puede tener más de 50 caracteres")]
        [Display(Name = "Vía de Administración")]
        public string? ViaAdministracion { get; set; }

        [StringLength(50, ErrorMessage = "El sitio de aplicación no puede tener más de 50 caracteres")]
        [Display(Name = "Sitio de Aplicación")]
        public string? SitioAplicacion { get; set; }

        // DATOS DEL RESPONSABLE
        [StringLength(255, ErrorMessage = "El nombre del vacunador no puede tener más de 255 caracteres")]
        [Display(Name = "Vacunador")]
        public string? Vacunador { get; set; }

        [StringLength(50, ErrorMessage = "El registro profesional no puede tener más de 50 caracteres")]
        [Display(Name = "Registro Profesional")]
        public string? RegistroProfesional { get; set; }

        // OBSERVACIONES Y NOTAS
        [StringLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [StringLength(1000, ErrorMessage = "Las notas finales no pueden tener más de 1000 caracteres")]
        [Display(Name = "Notas Finales")]
        public string? NotasFinales { get; set; }

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
        [NotMapped]
        public int EdadAlMomento 
        {
            get
            {
                var fechaReferencia = FechaAplicacion != default ? FechaAplicacion : DateTime.Now;
                var edad = fechaReferencia.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > fechaReferencia.AddYears(-edad).Date)
                    edad--;
                return edad;
            }
        }

        [NotMapped]
        public string NombreCompleto => NombresApellidos;

        [NotMapped]
        public string DocumentoCompleto => $"{TipoDocumento} {NumeroDocumento}";

        [NotMapped]
        public string VacunaCompleta => $"{Vacuna} - {NumeroDosis}";
    }
}