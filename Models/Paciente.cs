using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Highdmin.Models
{
    public class Paciente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La EPS es obligatoria")]
        [StringLength(100, ErrorMessage = "La EPS no puede tener más de 100 caracteres")]
        [Display(Name = "EPS")]
        public string Eps { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(20, ErrorMessage = "La identificación no puede tener más de 20 caracteres")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        [StringLength(10, ErrorMessage = "El tipo de identificación no puede tener más de 10 caracteres")]
        [Display(Name = "Tipo de Identificación")]
        public string TipoIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El primer nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Primer Nombre")]   
        public string PrimerNombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo nombre no puede tener más de 100 caracteres")]
        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;       
        [Required(ErrorMessage = "El segundo nombre es obligatorio")]

        [StringLength(100, ErrorMessage = "El primer apellido no puede tener más de 100 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El segundo apellido no puede tener más de 100 caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, ErrorMessage = "El género no puede tener más de 10 caracteres")]
        [Display(Name = "Genero")]
        public string Genero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(10, ErrorMessage = "El sexo no puede tener más de 10 caracteres")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; } 

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        // Propiedades calculadas
        [NotMapped]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}";

        [NotMapped]
        [Display(Name = "Edad")]
        public int Edad => DateTime.Now.Year - FechaNacimiento.Year - 
                          (DateTime.Now.DayOfYear < FechaNacimiento.DayOfYear ? 1 : 0);

        // Relaciones
        public virtual ICollection<RegistroVacunacion>? RegistrosVacunacion { get; set; }
    }
}