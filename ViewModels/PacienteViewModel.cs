using System.ComponentModel.DataAnnotations;
using Highdmin.Models;

namespace Highdmin.ViewModels
{
    public class PacienteViewModel
    {
        public int TotalPacientes { get; set; }
        public int PacientesConDatos { get; set; }
        public int CargasRealizadas { get; set; }
        public List<PacienteItemViewModel> Pacientes { get; set; } = new();
        public List<CargaHistorialItem> HistorialCargas { get; set; } = new();
    }

    public class PacienteItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "EPS")]
        public string Eps { get; set; } = string.Empty;

        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Display(Name = "Tipo Identificación")]
        public string TipoIdentificacion { get; set; } = string.Empty;

        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = string.Empty;

        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;

        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [Display(Name = "Genero")]
        public string Genero { get; set; } = string.Empty;

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}";

        [Display(Name = "Edad")]
        public int Edad => DateTime.Now.Year - FechaNacimiento.Year -
                          (DateTime.Now.DayOfYear < FechaNacimiento.DayOfYear ? 1 : 0);

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } 
    }

    public class PacienteCreateViewModel
    {
        [Required(ErrorMessage = "La EPS es obligatoria")]
        [StringLength(100, ErrorMessage = "La EPS no puede tener más de 100 caracteres")]
        [Display(Name = "EPS")]
        public string Eps { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        [StringLength(10, ErrorMessage = "El tipo de identificación no puede tener más de 10 caracteres")]
        [Display(Name = "Tipo Identificación")]
        public string TipoIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(20, ErrorMessage = "La identificación no puede tener más de 20 caracteres")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Primer Nombre son obligatorios")]
        [StringLength(100, ErrorMessage = "Los Primer Nombre no pueden tener más de 100 caracteres")]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Los Segundo Nombre no pueden tener más de 100 caracteres")]
        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Primer Apellido son obligatorios")]
        [StringLength(100, ErrorMessage = "Los Primer Apellido no pueden tener más de 100 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Los Segundo Apellido no pueden tener más de 100 caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(10, ErrorMessage = "El sexo no puede tener más de 10 caracteres")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

         [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, ErrorMessage = "El género no puede tener más de 10 caracteres")]
        [Display(Name = "Genero")]
        public string Genero { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; } 

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }

    public class PacienteEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La EPS es obligatoria")]
        [StringLength(100, ErrorMessage = "La EPS no puede tener más de 100 caracteres")]
        [Display(Name = "EPS")]
        public string Eps { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de identificación es obligatorio")]
        [StringLength(10, ErrorMessage = "El tipo de identificación no puede tener más de 10 caracteres")]
        [Display(Name = "Tipo Identificación")]
        public string TipoIdentificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La identificación es obligatoria")]
        [StringLength(20, ErrorMessage = "La identificación no puede tener más de 20 caracteres")]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Primer Nombre son obligatorios")]
        [StringLength(100, ErrorMessage = "Los Primer Nombre no pueden tener más de 100 caracteres")]
        [Display(Name = "Primer Nombre")]
        public string PrimerNombre { get; set; } = string.Empty;
 
        [StringLength(100, ErrorMessage = "Los Segundo Nombre no pueden tener más de 100 caracteres")]
        [Display(Name = "Segundo Nombre")]
        public string SegundoNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los Primer Apellido son obligatorios")]
        [StringLength(100, ErrorMessage = "Los Primer Apellido no pueden tener más de 100 caracteres")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Los Segundo Apellido no pueden tener más de 100 caracteres")]
        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(10, ErrorMessage = "El sexo no puede tener más de 10 caracteres")]
        [Display(Name = "Sexo")]
        public string Sexo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El género es obligatorio")]
        [StringLength(10, ErrorMessage = "El género no puede tener más de 10 caracteres")]
        [Display(Name = "Genero")]
        public string Genero { get; set; } = string.Empty;

        [StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El email no puede tener más de 255 caracteres")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "La dirección no puede tener más de 500 caracteres")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }
    }

    public class CargaHistorialItem
    {
        public string? Eps { get; set; } = string.Empty;
        public string Archivo { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public int Registros { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Acciones { get; set; } = string.Empty;
    }

    public class ImportarPacientesViewModel
    { 
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        [Display(Name = "Sobrescribir datos existentes")]
        public bool SobrescribirDatos { get; set; } = false;

        [Display(Name = "EPS")]
        [StringLength(100)]
        public string? EpsFilter { get; set; }

        public List<PacienteItemViewModel>? PacientesCargados { get; set; }
    }
}