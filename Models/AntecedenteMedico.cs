using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class AntecedenteMedico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("RegistroVacunacion")]
        public int RegistroVacunacionId { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "El tipo de antecedente es obligatorio")]
        [StringLength(100, ErrorMessage = "El tipo no puede tener más de 100 caracteres")]
        [Display(Name = "Tipo de Antecedente")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Las observaciones no pueden tener más de 500 caracteres")]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Modificación")]
        public DateTime? FechaModificacion { get; set; }

        [Display(Name = "Número Documento Paciente")]
        public string NumeroDocumentoPaciente { get; set; } = string.Empty;

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }

        // Navegación
        public virtual RegistrosVacunacion? RegistroVacunacion { get; set; }
    }
}