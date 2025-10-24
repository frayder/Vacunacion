using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    // ViewModel para el Index - Lista de insumos
    public class InsumoIndexViewModel
    {
        public List<InsumoItemViewModel> Insumos { get; set; } = new List<InsumoItemViewModel>();
        public int TotalInsumos => Insumos.Count;
        
        // Propiedades de permisos
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    // ViewModel para un insumo individual (Details)
    public class InsumoViewModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? RangoDosis { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<ConfiguracionRangoInsumoViewModel> ConfiguracionesRango { get; set; } = new List<ConfiguracionRangoInsumoViewModel>();
        
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
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

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

        // Propiedades temporales para configuración de rangos de edad y dosis (para la interfaz)
        [Display(Name = "Descripción del Rango")]
        public string? DescripcionRango { get; set; }

        [Display(Name = "Edad Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int? EdadMinima { get; set; }

        [Display(Name = "Edad Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int? EdadMaxima { get; set; }

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        // Unidades de medida para la edad mínima y máxima
        [Display(Name = "Unidad de Medida Edad Mínima")]
        public string? UnidadMedidaEdadMinima { get; set; }

        [Display(Name = "Unidad de Medida Edad Máxima")]
        public string? UnidadMedidaEdadMaxima { get; set; }

        // Lista de configuraciones de rangos
        public List<ConfiguracionRangoInsumoViewModel> ConfiguracionesRango { get; set; } = new List<ConfiguracionRangoInsumoViewModel>();
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

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Propiedades temporales para configuración de rangos (para la interfaz)
        [Display(Name = "Descripción del Rango")]
        public string? DescripcionRango { get; set; }

        [Display(Name = "Edad Mínima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
        public int? EdadMinima { get; set; }

        [Display(Name = "Edad Máxima")]
        [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
        public int? EdadMaxima { get; set; }

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Display(Name = "Unidad de Medida Edad Mínima")]
        public string? UnidadMedidaEdadMinima { get; set; }

        [Display(Name = "Unidad de Medida Edad Máxima")]
        public string? UnidadMedidaEdadMaxima { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        public string? RangoDosis { get; set; }

        // Lista de configuraciones de rangos
        public List<ConfiguracionRangoInsumoViewModel> ConfiguracionesRango { get; set; } = new List<ConfiguracionRangoInsumoViewModel>();
    }

    // public class ConfiguracionRangoInsumoViewModel
    // {
    //     public int Id { get; set; }

    //     [Required(ErrorMessage = "La edad mínima es obligatoria")]
    //     [Display(Name = "Edad Mínima")]
    //     [Range(0, int.MaxValue, ErrorMessage = "La edad mínima debe ser un número positivo")]
    //     public int EdadMinima { get; set; }

    //     [Required(ErrorMessage = "La edad máxima es obligatoria")]
    //     [Display(Name = "Edad Máxima")]
    //     [Range(0, int.MaxValue, ErrorMessage = "La edad máxima debe ser un número positivo")]
    //     public int EdadMaxima { get; set; }

    //     [Required(ErrorMessage = "La unidad de medida de edad mínima es obligatoria")]
    //     [Display(Name = "Unidad de Medida Edad Mínima")]
    //     public string UnidadMedidaEdadMinima { get; set; } = string.Empty;

    //     [Required(ErrorMessage = "La unidad de medida de edad máxima es obligatoria")]
    //     [Display(Name = "Unidad de Medida Edad Máxima")]
    //     public string UnidadMedidaEdadMaxima { get; set; } = string.Empty;

    //     [Display(Name = "Dosis")]
    //     public string? Dosis { get; set; }

    //     [Display(Name = "Descripción del Rango")]
    //     public string DescripcionRango { get; set; } = string.Empty;

    //     [Display(Name = "Estado")]
    //     public bool Estado { get; set; } = true;
    // }
}