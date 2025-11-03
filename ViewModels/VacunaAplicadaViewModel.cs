using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class VacunaAplicadaViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Registro de Vacunación")]
        public int RegistroVacunacionId { get; set; }

        [Required(ErrorMessage = "El insumo (vacuna) es obligatorio")]
        [Display(Name = "Insumo ID")]
        public int InsumoId { get; set; }

        [Required(ErrorMessage = "El nombre de la vacuna es obligatorio")]
        [Display(Name = "Nombre de la Vacuna")]
        public string NombreVacuna { get; set; } = string.Empty;

        [Display(Name = "Dosis")]
        public string? Dosis { get; set; }

        [Required(ErrorMessage = "El lote de la vacuna es obligatorio")]
        [Display(Name = "Lote de la Vacuna")]
        public string LoteVacuna { get; set; } = string.Empty;

        [Display(Name = "Jeringa")]
        public string? Jeringa { get; set; }

        [Display(Name = "Lote Jeringa")]
        public string? LoteJeringa { get; set; }

        [Display(Name = "Lote Diluyente")]
        public string? LoteDiluyente { get; set; }

        [Display(Name = "Gotero")]
        public string? Gotero { get; set; }

        [Display(Name = "Número de Frascos")]
        public int? NumeroFrascos { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Marcar como Perdida")]
        public bool MarcarComoPerdida { get; set; } = false;

        [Display(Name = "Motivo de Pérdida")]
        public string? MotivoPerdida { get; set; }

        [Display(Name = "Fecha de Aplicación")]
        [DataType(DataType.Date)]
        public DateTime FechaAplicacion { get; set; } = DateTime.UtcNow;

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha Formateada")]
        public string? FechaFormateada { get; set; }

        // Para el mapeo desde el frontend
        public long? ClienteId { get; set; } // El ID temporal que viene del frontend (Date.now())
    }

    public class ListaVacunasAplicadasViewModel
    {
        public int RegistroVacunacionId { get; set; }
        public List<VacunaAplicadaViewModel> VacunasAplicadas { get; set; } = new();
        public VacunaAplicadaViewModel NuevaVacuna { get; set; } = new();

        // Propiedades para permisos
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }
}