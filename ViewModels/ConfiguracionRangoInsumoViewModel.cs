using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class ConfiguracionRangoInsumoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La edad mínima es obligatoria")]
        [Display(Name = "Edad Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int EdadMinima { get; set; }

        [Required(ErrorMessage = "La edad máxima es obligatoria")]
        [Display(Name = "Edad Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int EdadMaxima { get; set; }

        [Required(ErrorMessage = "La unidad de medida de edad mínima es obligatoria")]
        [Display(Name = "Unidad de Medida Edad Mínima")]
        public string UnidadMedidaEdadMinima { get; set; } = string.Empty;

        [Required(ErrorMessage = "La unidad de medida de edad máxima es obligatoria")]
        [Display(Name = "Unidad de Medida Edad Máxima")]
        public string UnidadMedidaEdadMaxima { get; set; } = string.Empty;

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Display(Name = "Descripción del Rango")]
        public string DescripcionRango { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}