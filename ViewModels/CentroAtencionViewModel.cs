using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class CentroAtencionViewModel
    {
        public int TotalCentros { get; set; }
        public int CentrosActivos { get; set; }
        public int CentrosInactivos { get; set; }
        public List<CentroAtencionItemViewModel> CentrosAtencion { get; set; } = new();
    }

    public class CentroAtencionItemViewModel : ICatalogoItem
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

        [StringLength(500, ErrorMessage = "El tipo no puede tener más de 500 caracteres")]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Implementación de ICatalogoItem - mapea Tipo a Descripcion para compatibilidad
        public string? Descripcion 
        { 
            get => Tipo; 
            set => Tipo = value; 
        }

        // Propiedades calculadas para mostrar en la vista
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
        public string EstadoClass => Estado ? "badge bg-success" : "badge bg-danger";
    }

    public class CentroAtencionCreateViewModel
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [StringLength(10, ErrorMessage = "El código no puede tener más de 10 caracteres")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(255, ErrorMessage = "El nombre no puede tener más de 255 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "El tipo no puede tener más de 500 caracteres")]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }

    public class CentroAtencionEditViewModel
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

        [StringLength(500, ErrorMessage = "El tipo no puede tener más de 500 caracteres")]
        [Display(Name = "Tipo")]
        public string? Tipo { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }
    }
}