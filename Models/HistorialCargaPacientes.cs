using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    [Table("HistorialCargaPacientes")]
    public class HistorialCargaPacientes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCarga { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string Eps { get; set; } = string.Empty;

        [Required]
        public string Usuario { get; set; } = string.Empty;

        public int TotalCargados { get; set; }

        public int TotalExistentes { get; set; }

        [StringLength(255)]
        public string? ArchivoNombre { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }
    }
}
