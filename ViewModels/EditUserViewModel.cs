using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede tener más de 100 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio")]
        [Display(Name = "Rol")]
        public int RoleId { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
        [Display(Name = "Nueva Contraseña")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string? ConfirmNewPassword { get; set; }

        // Campos de solo lectura para información adicional
        public string PasswordHash { get; set; } = string.Empty;
        public string? PasswordSalt { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public int EmpresaId { get; set; }
        public bool IsActive { get; set; }
    }
}