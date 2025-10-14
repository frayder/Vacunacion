using System.ComponentModel.DataAnnotations;

namespace Highdmin.ViewModels
{
    public class ImportarAseguradoraViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<AseguradoraItemViewModel>? AseguradorasCargadas { get; set; }
    }

    public class ImportarCentroAtencionViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<CentroAtencionItemViewModel>? CentrosCargados { get; set; }
    }

    public class ImportarCondicionUsuariaViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<CondicionUsuariaItemViewModel>? CondicionesCargadas { get; set; }
    }

    public class ImportarInsumoViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<InsumoItemViewModel>? InsumosCargados { get; set; }
    }

    public class ImportarPertenenciaEtnicaViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<PertenenciaEtnicaItemViewModel>? PertenenciasCargadas { get; set; }
    }

    public class ImportarRegimenAfiliacionViewModel
    {
        [Display(Name = "Archivo Excel")]
        public IFormFile ArchivoExcel { get; set; } = null!;

        public List<RegimenAfiliacionItemViewModel>? RegimenesCargados { get; set; }
    }
}