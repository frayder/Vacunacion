using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    // ViewModel genérico para entidades que tienen la misma estructura
    public class CatalogoGenericoViewModel<T> where T : class
    {
        public int TotalItems { get; set; }
        public int ItemsActivos { get; set; }
        public int ItemsInactivos { get; set; }
        public List<T> Items { get; set; } = new();
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string IconoHeader { get; set; } = "ri-settings-line";
        public string ControllerName { get; set; } = string.Empty;
    }

    // Interface para elementos de catálogo genérico
    public interface ICatalogoItem
    {
        int Id { get; set; }
        string Codigo { get; set; }
        string Nombre { get; set; }
        string? Descripcion { get; set; }
        bool Estado { get; set; }
        DateTime FechaCreacion { get; set; }
        string EstadoTexto { get; }
        string EstadoClass { get; }
    }

    // Base class para ViewModels de catálogo
    public abstract class CatalogoItemViewModelBase : ICatalogoItem
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

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Propiedades calculadas para mostrar en la vista
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
        public string EstadoClass => Estado ? "badge bg-success" : "badge bg-danger";
    }
}