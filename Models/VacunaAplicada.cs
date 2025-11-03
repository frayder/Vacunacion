using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class VacunaAplicada
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("RegistroVacunacion")]
        public int RegistroVacunacionId { get; set; }

        [Required(ErrorMessage = "El insumo (vacuna) es obligatorio")]
        [Display(Name = "Insumo/Vacuna")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre de la vacuna no puede tener más de 200 caracteres")]
        [Display(Name = "Nombre de la Vacuna")]
        public string NombreVacuna { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "La dosis no puede tener más de 50 caracteres")]
        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Required(ErrorMessage = "El lote de la vacuna es obligatorio")]
        [StringLength(100, ErrorMessage = "El lote no puede tener más de 100 caracteres")]
        [Display(Name = "Lote de la Vacuna")]
        public string LoteVacuna { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La jeringa no puede tener más de 100 caracteres")]
        [Display(Name = "Jeringa")]
        public string? Jeringa { get; set; }

        [StringLength(100, ErrorMessage = "El lote de jeringa no puede tener más de 100 caracteres")]
        [Display(Name = "Lote Jeringa")]
        public string? LoteJeringa { get; set; }

        [StringLength(100, ErrorMessage = "El lote de diluyente no puede tener más de 100 caracteres")]
        [Display(Name = "Lote Diluyente")]
        public string? LoteDiluyente { get; set; }

        [StringLength(100, ErrorMessage = "El gotero no puede tener más de 100 caracteres")]
        [Display(Name = "Gotero")]
        public string? Gotero { get; set; }

        [Display(Name = "Número de Frascos")]
        public int? NumeroFrascos { get; set; }

        [StringLength(1000, ErrorMessage = "Las observaciones no pueden tener más de 1000 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Marcar como Perdida")]
        public bool MarcarComoPerdida { get; set; } = false;

        [StringLength(200, ErrorMessage = "El motivo de pérdida no puede tener más de 200 caracteres")]
        [Display(Name = "Motivo de Pérdida")]
        public string? MotivoPerdida { get; set; }

        [Display(Name = "Fecha de Aplicación")]
        [DataType(DataType.Date)]
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Número Documento Paciente")]
        public string NumeroDocumentoPaciente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }

        [ForeignKey("InsumoId")]
        public virtual Insumo? Insumo { get; set; }

        // Navegación
        public virtual RegistrosVacunacion? RegistroVacunacion { get; set; }
    }
}