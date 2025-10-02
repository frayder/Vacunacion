using System.ComponentModel.DataAnnotations;

namespace Highdmin.Models
{
    public class Entrada
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es obligatoria")]
        [Display(Name = "Fecha de Entrada")]
        public DateTime FechaEntrada { get; set; } = DateTime.Now;

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
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Insumo? Insumo { get; set; }
        public virtual User? Usuario { get; set; }
    }
}