using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class Insumo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(10, ErrorMessage = "El código no puede tener más de 10 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener más de 255 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo es obligatorio")]
        [StringLength(50, ErrorMessage = "El tipo no puede tener más de 50 caracteres")]
        [Display(Name = "Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Rango/Dosis")]
        public string? RangoDosis { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La empresa es obligatoria")]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        [ForeignKey("EmpresaId")]
        public virtual Empresa? Empresa { get; set; }

        // Unidades de medida para la edad mínima y máxima
        [Required(ErrorMessage = "la unidad de medida es obligatoria")]
        [Display(Name = "Unidad de Medida")]   
        public string UnidadMedidaEdadMinima { get; set; }

        [Required(ErrorMessage = "la unidad de medida es obligatoria")]
        [Display(Name = "Unidad de Medida ")]
        public string UnidadMedidaEdadMaxima { get; set; }

        [Required(ErrorMessage = "la edad mínima es obligatoria")]
        [Display(Name = "Edad Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int EdadMinima { get; set; }

        [Required(ErrorMessage = "la edad máxima es obligatoria")]
        [Display(Name = "Edad Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int EdadMaxima { get; set; }
    }
}