using System.ComponentModel.DataAnnotations;

namespace Highdmin.Models
{
    public class Empresa
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

        [Required(ErrorMessage = "La razón social es obligatoria")]
        [StringLength(255, ErrorMessage = "La razón social no puede tener más de 255 caracteres")]
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El NIT es obligatorio")]
        [StringLength(20, ErrorMessage = "El NIT no puede tener más de 20 caracteres")]
        [Display(Name = "NIT")]
        public string Nit { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La dirección no puede tener más de 500 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        public virtual ICollection<User> Usuarios { get; set; } = new List<User>();
        public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();
        public virtual ICollection<RegistrosVacunacion> RegistrosVacunacion { get; set; } = new List<RegistrosVacunacion>();
        public virtual ICollection<CentroAtencion> CentrosAtencion { get; set; } = new List<CentroAtencion>();
        public virtual ICollection<Insumo> Insumos { get; set; } = new List<Insumo>();
        public virtual ICollection<Entrada> Entradas { get; set; } = new List<Entrada>();
    }
}