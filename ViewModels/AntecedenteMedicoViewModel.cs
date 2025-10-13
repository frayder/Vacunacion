using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class ListaAntecedentesViewModel
    {
        public int RegistroVacunacionId { get; set; }
        public List<AntecedenteMedicoViewModel> Antecedentes { get; set; } = new();
        public AntecedenteMedicoViewModel NuevoAntecedente { get; set; } = new();

        // Propiedades para permisos
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; } 
    }
    
    public class AntecedenteMedicoViewModel
    {
        public int Id { get; set; }
        
        public int RegistroVacunacionId { get; set; }

        [Required(ErrorMessage = "La fecha de registro es obligatoria")]
        [Display(Name = "Fecha de Registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Numero Documento Paciente es obligatoria")]
        [Display(Name = "Numero Documento Paciente")]
        public int NumeroDocumentoPaciente { get; set; } = 0;

        [Required(ErrorMessage = "El tipo de antecedente es obligatorio")]
        [Display(Name = "Tipo de Antecedente")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public bool Activo { get; set; } = true;
    } 
}