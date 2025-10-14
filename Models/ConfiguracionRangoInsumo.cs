using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class ConfiguracionRangoInsumo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InsumoId { get; set; }

        [ForeignKey("InsumoId")]
        public virtual Insumo? Insumo { get; set; }

        [Required(ErrorMessage = "La edad mínima es obligatoria")]
        [Display(Name = "Edad Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int EdadMinima { get; set; }

        [Required(ErrorMessage = "La edad máxima es obligatoria")]
        [Display(Name = "Edad Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int EdadMaxima { get; set; }

        [Required(ErrorMessage = "La unidad de medida de edad mínima es obligatoria")]
        [StringLength(50)]
        [Display(Name = "Unidad de Medida Edad Mínima")]
        public string UnidadMedidaEdadMinima { get; set; } = string.Empty;

        [Required(ErrorMessage = "La unidad de medida de edad máxima es obligatoria")]
        [StringLength(50)]
        [Display(Name = "Unidad de Medida Edad Máxima")]
        public string UnidadMedidaEdadMaxima { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Display(Name = "Descripción del Rango")]
        public string DescripcionRango { get; set; } = string.Empty;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}