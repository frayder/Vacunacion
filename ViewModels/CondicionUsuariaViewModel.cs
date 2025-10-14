using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class CondicionUsuariaViewModel
    {
        public int TotalCondiciones { get; set; }
        public int CondicionesActivas { get; set; }
        public int CondicionesInactivas { get; set; }
        public List<CondicionUsuariaItemViewModel> CondicionesUsuarias { get; set; } = new();

        // Propiedades para permisos
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; } 
    }

    public class CondicionUsuariaItemViewModel : CatalogoItemViewModelBase
    {
        // Hereda todas las propiedades de CatalogoItemViewModelBase
    }

    public class CondicionUsuariaCreateViewModel
    {
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
    }

    public class CondicionUsuariaEditViewModel : CondicionUsuariaCreateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }
    }
}