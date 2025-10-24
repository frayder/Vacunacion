using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Highdmin.ViewModels
{
    public class EntradaViewModel
    {
        public int TotalEntradas { get; set; }
        public int EntradasDelMes { get; set; }
        public int InsumosDiferentes { get; set; }
        public int UsuariosActivos { get; set; }
        public List<EntradaItemViewModel> Entradas { get; set; } = new();

        // Propiedades para permisos
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; } 
    }

    public class EntradaItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Fecha")]
        public DateTime FechaEntrada { get; set; }

        [Display(Name = "Insumo")]
        public string InsumoNombre { get; set; } = string.Empty;

        [Display(Name = "Código Insumo")]
        public string InsumoCodigo { get; set; } = string.Empty;

        [Display(Name = "Tipo")]
        public string InsumoTipo { get; set; } = string.Empty;

        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Usuario")]
        public string UsuarioNombre { get; set; } = string.Empty;

        [Display(Name = "Mes")]
        public string Mes { get; set; } = string.Empty;

        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        // Propiedades calculadas
        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
        public string EstadoClass => Estado ? "badge bg-success" : "badge bg-danger";
        
        public string TipoBadgeClass => InsumoTipo?.ToLower() switch
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

    public class EntradaCreateViewModel
    {
        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        [Display(Name = "Fecha de Entrada")]
        public DateTime FechaEntrada { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "El insumo es obligatorio")]
        [Display(Name = "Insumo")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Display(Name = "Mes")]
        public string Mes { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Las notas no pueden tener más de 500 caracteres")]
        [Display(Name = "Notas (Opcional)")]
        public string? Notas { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        // Para los dropdowns
        public List<SelectListItem> Insumos { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();
        public List<SelectListItem> Meses { get; set; } = new();
    }

    public class EntradaEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        [Display(Name = "Fecha de Entrada")]
        public DateTime FechaEntrada { get; set; }

        [Required(ErrorMessage = "El insumo es obligatorio")]
        [Display(Name = "Insumo")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El mes es obligatorio")]
        [Display(Name = "Mes")]
        public string Mes { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Las notas no pueden tener más de 500 caracteres")]
        [Display(Name = "Notas")]
        public string? Notas { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; }

        // Para los dropdowns
        public List<SelectListItem> Insumos { get; set; } = new();
        public List<SelectListItem> Usuarios { get; set; } = new();
        public List<SelectListItem> Meses { get; set; } = new();
    }
}