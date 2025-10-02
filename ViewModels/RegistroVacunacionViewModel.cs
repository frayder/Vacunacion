using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class RegistroVacunacionViewModel
    {
        public int TotalRegistros { get; set; }
        public int EsquemasCompletos { get; set; }
        public int EsquemasIncompletos { get; set; }
        public int Pendientes { get; set; }
        public List<RegistroVacunacionItemViewModel> Registros { get; set; } = new();
    }

    public class RegistroVacunacionItemViewModel
    {
        public int Id { get; set; }
        
        [Display(Name = "Paciente")]
        [Required(ErrorMessage = "El nombre del paciente es obligatorio")]
        public string Paciente { get; set; } = string.Empty;
        
        [Display(Name = "Documento")]
        [Required(ErrorMessage = "El documento es obligatorio")]
        public string Documento { get; set; } = string.Empty;
        
        [Display(Name = "Edad")]
        [Range(0, 120, ErrorMessage = "La edad debe estar entre 0 y 120 años")]
        public int Edad { get; set; }
        
        [Display(Name = "Vacunas Aplicadas")]
        public string VacunasAplicadas { get; set; } = string.Empty;
        
        [Display(Name = "Última Vacuna")]
        public string UltimaVacuna { get; set; } = string.Empty;
        
        [Display(Name = "Estado")]
        public string Estado { get; set; } = string.Empty;
        
        [Display(Name = "Fecha Registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        
        // Campos adicionales para el formulario de nuevo registro
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }
        
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;
        
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;
        
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;
        
        [Display(Name = "Centro de Atención")]
        public string CentroAtencion { get; set; } = string.Empty;
        
        [Display(Name = "Tipo de Vacuna")]
        public string TipoVacuna { get; set; } = string.Empty;
        
        [Display(Name = "Dosis")]
        public string Dosis { get; set; } = string.Empty;
        
        [Display(Name = "Fecha de Aplicación")]
        [DataType(DataType.Date)]
        public DateTime? FechaAplicacion { get; set; }
        
        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; } = string.Empty;
        
        [Display(Name = "Condición Usuario/a")]
        public int? CondicionUsuariaId { get; set; }
    }
}