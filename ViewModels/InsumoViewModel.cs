using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class InsumoViewModel
    {
        public int TotalInsumos { get; set; }
        public int InsumosActivos { get; set; }
        public int InsumosInactivos { get; set; }
        public List<InsumoItemViewModel> Insumos { get; set; } = new();
    }

    public class InsumoItemViewModel : ICatalogoItem
    {
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

        // Propiedades calculadas para mostrar en la vista
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
        public string EstadoClass => Estado ? "badge bg-success" : "badge bg-danger";

        // Propiedades para mostrar el tipo con badge
        public string TipoBadgeClass => Tipo?.ToLower() switch
        {
            "vacuna" => "badge bg-primary",
            "jeringa" => "badge bg-info", 
            "diluyente" => "badge bg-warning",
            "gotero" => "badge bg-success",
            "carnet" => "badge bg-secondary",
            "tarjeta" => "badge bg-dark",
            _ => "badge bg-light text-dark"
        };
    }

    public class InsumoCreateViewModel
    {
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

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Propiedades para configuración de rangos de edad y dosis
        [Display(Name = "Descripción del Rango")]
        public string? DescripcionRango { get; set; }

        [Display(Name = "Edad Mínima (días)")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int? EdadMinimaDias { get; set; }

        [Display(Name = "Edad Máxima (días)")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int? EdadMaximaDias { get; set; }

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }
    }

    public class InsumoEditViewModel
    {
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
        public DateTime FechaCreacion { get; set; }
    }

    public class RangoEdadDosis
    {
        [Display(Name = "Descripción del Rango")]
        public string? Descripcion { get; set; }

        [Display(Name = "Edad Mínima (días)")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int? EdadMinimaDias { get; set; }

        [Display(Name = "Edad Máxima (días)")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int? EdadMaximaDias { get; set; }

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }
    }
}