namespace Highdmin.Services
{
    public interface IEntityConfigurationService
    {
        ImportConfiguration<T> GetImportConfiguration<T>() where T : class;
        ExportConfiguration<T> GetExportConfiguration<T>() where T : class;
    }

    public class EntityConfigurationService : IEntityConfigurationService
    {
        private readonly Dictionary<Type, object> _importConfigurations = new();
        private readonly Dictionary<Type, object> _exportConfigurations = new();

        public EntityConfigurationService()
        {
            ConfigureTipoCarnet();
            ConfigureAseguradora();
            ConfigureCentroAtencion();
            ConfigureCondicionUsuaria();
            ConfigureInsumo();
            ConfigurePertenenciaEtnica();
            ConfigureRegimenAfiliacion();
        }

        public ImportConfiguration<T> GetImportConfiguration<T>() where T : class
        {
            var type = typeof(T);
            if (_importConfigurations.ContainsKey(type))
            {
                return (ImportConfiguration<T>)_importConfigurations[type];
            }
            throw new InvalidOperationException($"No import configuration found for type {type.Name}");
        }

        public ExportConfiguration<T> GetExportConfiguration<T>() where T : class
        {
            var type = typeof(T);
            if (_exportConfigurations.ContainsKey(type))
            {
                return (ExportConfiguration<T>)_exportConfigurations[type];
            }
            throw new InvalidOperationException($"No export configuration found for type {type.Name}");
        }

        #region TipoCarnet Configuration
        private void ConfigureTipoCarnet()
        {
            var importConfig = new ImportConfiguration<ViewModels.TipoCarnetItemViewModel>
            {
                SheetName = "Tipos de Carnet",
                ColumnMappings = new Dictionary<string, string>
                {
                    { "Codigo", "Código" },
                    { "Nombre", "Nombre" },
                    { "Descripcion", "Descripción" },
                    { "Estado", "Estado" }
                },
                ValidationValues = new Dictionary<string, string[]>
                {
                    { "Estado", new[] { "Activo", "Inactivo" } }
                },
                ExampleData = new List<Dictionary<string, string>>
                {
                    new() { { "Codigo", "CC" }, { "Nombre", "Cédula de Ciudadanía" }, { "Descripcion", "Documento de identificación para ciudadanos colombianos" }, { "Estado", "Activo" } },
                    new() { { "Codigo", "TI" }, { "Nombre", "Tarjeta de Identidad" }, { "Descripcion", "Documento de identificación para menores de edad" }, { "Estado", "Activo" } }
                },
                MapFunction = MapTipoCarnetFromRow,
                CustomValidation = ValidateTipoCarnet
            };

            var exportConfig = new ExportConfiguration<Models.TipoCarnet>
            {
                SheetName = "Tipos de Carnet",
                FileName = "TiposCarnet",
                ColumnMappings = new Dictionary<string, string>
                {
                    { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" }
                },
                CustomMapping = item => new Dictionary<string, object>
                {
                    { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, 
                    { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") }
                }
            };

            _importConfigurations[typeof(ViewModels.TipoCarnetItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.TipoCarnet)] = exportConfig;
        }

        private ViewModels.TipoCarnetItemViewModel? MapTipoCarnetFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.TipoCarnetItemViewModel
            {
                Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow
            };
        }

        private List<string> ValidateTipoCarnet(ViewModels.TipoCarnetItemViewModel item, int rowNumber)
        {
            var errors = new List<string>();
            if (item.Codigo.Length > 10) errors.Add($"Fila {rowNumber}: El código no puede tener más de 10 caracteres");
            if (item.Nombre.Length > 255) errors.Add($"Fila {rowNumber}: El nombre no puede tener más de 255 caracteres");
            if (!string.IsNullOrEmpty(item.Descripcion) && item.Descripcion.Length > 500) errors.Add($"Fila {rowNumber}: La descripción no puede tener más de 500 caracteres");
            return errors;
        }
        #endregion

        #region Aseguradora Configuration
        private void ConfigureAseguradora()
        {
            var importConfig = new ImportConfiguration<ViewModels.AseguradoraItemViewModel>
            {
                SheetName = "Aseguradoras",
                ColumnMappings = new Dictionary<string, string>
                {
                    { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }
                },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } } },
                ExampleData = new List<Dictionary<string, string>>
                {
                    new() { { "Codigo", "EPS001" }, { "Nombre", "Nueva EPS" }, { "Descripcion", "Entidad Promotora de Salud" }, { "Estado", "Activo" } },
                    new() { { "Codigo", "EPS002" }, { "Nombre", "Sanitas EPS" }, { "Descripcion", "Entidad Promotora de Salud Sanitas" }, { "Estado", "Activo" } }
                },
                MapFunction = MapAseguradoraFromRow,
                CustomValidation = ValidateAseguradora
            };

            var exportConfig = new ExportConfiguration<Models.Aseguradora>
            {
                SheetName = "Aseguradoras", FileName = "Aseguradoras",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object>
                { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.AseguradoraItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.Aseguradora)] = exportConfig;
        }

        private ViewModels.AseguradoraItemViewModel? MapAseguradoraFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.AseguradoraItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidateAseguradora(ViewModels.AseguradoraItemViewModel item, int rowNumber)
        {
            var errors = new List<string>();
            if (item.Codigo.Length > 10) errors.Add($"Fila {rowNumber}: El código no puede tener más de 10 caracteres");
            if (item.Nombre.Length > 255) errors.Add($"Fila {rowNumber}: El nombre no puede tener más de 255 caracteres");
            return errors;
        }
        #endregion

        #region CentroAtencion Configuration
        private void ConfigureCentroAtencion()
        {
            var importConfig = new ImportConfiguration<ViewModels.CentroAtencionItemViewModel>
            {
                SheetName = "Centros de Atención",
                ColumnMappings = new Dictionary<string, string>
                { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Tipo", "Tipo" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" } },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } }, { "Tipo", new[] { "Hospital", "Clínica", "Puesto de Salud", "Centro de Salud" } } },
                ExampleData = new List<Dictionary<string, string>>
                {
                    new() { { "Codigo", "HSP001" }, { "Nombre", "Hospital Central" }, { "Tipo", "Hospital" }, { "Descripcion", "Hospital de alta complejidad" }, { "Estado", "Activo" } }
                },
                MapFunction = MapCentroAtencionFromRow,
                CustomValidation = ValidateCentroAtencion
            };

            var exportConfig = new ExportConfiguration<Models.CentroAtencion>
            {
                SheetName = "Centros de Atención", FileName = "CentrosAtencion",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Tipo", "Tipo" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object> { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Tipo", item.Tipo }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.CentroAtencionItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.CentroAtencion)] = exportConfig;
        }

        private ViewModels.CentroAtencionItemViewModel? MapCentroAtencionFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.CentroAtencionItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Tipo = rowData["Tipo"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidateCentroAtencion(ViewModels.CentroAtencionItemViewModel item, int rowNumber)
        {
            var errors = new List<string>();
            if (item.Codigo.Length > 20) errors.Add($"Fila {rowNumber}: El código no puede tener más de 20 caracteres");
            if (item.Nombre.Length > 255) errors.Add($"Fila {rowNumber}: El nombre no puede tener más de 255 caracteres");
            return errors;
        }
        #endregion

        #region CondicionUsuaria Configuration 
        private void ConfigureCondicionUsuaria()
        {
            var importConfig = new ImportConfiguration<ViewModels.CondicionUsuariaItemViewModel>
            {
                SheetName = "Condiciones Usuarias",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" } },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } } },
                ExampleData = new List<Dictionary<string, string>> { new() { { "Codigo", "COND001" }, { "Nombre", "Gestante" }, { "Descripcion", "Mujer en estado de gestación" }, { "Estado", "Activo" } } },
                MapFunction = MapCondicionUsuariaFromRow, CustomValidation = ValidateCondicionUsuaria
            };

            var exportConfig = new ExportConfiguration<Models.CondicionUsuaria>
            {
                SheetName = "Condiciones Usuarias", FileName = "CondicionesUsuarias",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object> { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.CondicionUsuariaItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.CondicionUsuaria)] = exportConfig;
        }

        private ViewModels.CondicionUsuariaItemViewModel? MapCondicionUsuariaFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.CondicionUsuariaItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidateCondicionUsuaria(ViewModels.CondicionUsuariaItemViewModel item, int rowNumber) => new();
        #endregion

        #region Insumo Configuration
        private void ConfigureInsumo()
        {
            var importConfig = new ImportConfiguration<ViewModels.InsumoItemViewModel>
            {
                SheetName = "Insumos",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" } },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } } },
                ExampleData = new List<Dictionary<string, string>> { new() { { "Codigo", "VAC001" }, { "Nombre", "Vacuna COVID-19" }, { "Descripcion", "Vacuna contra el coronavirus" }, { "Estado", "Activo" } } },
                MapFunction = MapInsumoFromRow, CustomValidation = ValidateInsumo
            };

            var exportConfig = new ExportConfiguration<Models.Insumo>
            {
                SheetName = "Insumos", FileName = "Insumos",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object> { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.InsumoItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.Insumo)] = exportConfig;
        }

        private ViewModels.InsumoItemViewModel? MapInsumoFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.InsumoItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidateInsumo(ViewModels.InsumoItemViewModel item, int rowNumber) => new();
        #endregion

        #region PertenenciaEtnica Configuration
        private void ConfigurePertenenciaEtnica()
        {
            var importConfig = new ImportConfiguration<ViewModels.PertenenciaEtnicaItemViewModel>
            {
                SheetName = "Pertenencias Étnicas",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" } },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } } },
                ExampleData = new List<Dictionary<string, string>> { new() { { "Codigo", "MEST" }, { "Nombre", "Mestizo" }, { "Descripcion", "Persona de origen mestizo" }, { "Estado", "Activo" } } },
                MapFunction = MapPertenenciaEtnicaFromRow, CustomValidation = ValidatePertenenciaEtnica
            };

            var exportConfig = new ExportConfiguration<Models.PertenenciaEtnica>
            {
                SheetName = "Pertenencias Étnicas", FileName = "PertenenciasEtnicas",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object> { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.PertenenciaEtnicaItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.PertenenciaEtnica)] = exportConfig;
        }

        private ViewModels.PertenenciaEtnicaItemViewModel? MapPertenenciaEtnicaFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.PertenenciaEtnicaItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidatePertenenciaEtnica(ViewModels.PertenenciaEtnicaItemViewModel item, int rowNumber) => new();
        #endregion

        #region RegimenAfiliacion Configuration
        private void ConfigureRegimenAfiliacion()
        {
            var importConfig = new ImportConfiguration<ViewModels.RegimenAfiliacionItemViewModel>
            {
                SheetName = "Regímenes de Afiliación",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" } },
                ValidationValues = new Dictionary<string, string[]> { { "Estado", new[] { "Activo", "Inactivo" } } },
                ExampleData = new List<Dictionary<string, string>> { new() { { "Codigo", "CONT" }, { "Nombre", "Contributivo" }, { "Descripcion", "Régimen contributivo de salud" }, { "Estado", "Activo" } } },
                MapFunction = MapRegimenAfiliacionFromRow, CustomValidation = ValidateRegimenAfiliacion
            };

            var exportConfig = new ExportConfiguration<Models.RegimenAfiliacion>
            {
                SheetName = "Regímenes de Afiliación", FileName = "RegimenesAfiliacion",
                ColumnMappings = new Dictionary<string, string> { { "Codigo", "Código" }, { "Nombre", "Nombre" }, { "Descripcion", "Descripción" }, { "Estado", "Estado" }, { "FechaCreacion", "Fecha Creación" } },
                CustomMapping = item => new Dictionary<string, object> { { "Codigo", item.Codigo }, { "Nombre", item.Nombre }, { "Descripcion", item.Descripcion ?? "" }, { "Estado", item.Estado ? "Activo" : "Inactivo" }, { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") } }
            };

            _importConfigurations[typeof(ViewModels.RegimenAfiliacionItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.RegimenAfiliacion)] = exportConfig;
        }

        private ViewModels.RegimenAfiliacionItemViewModel? MapRegimenAfiliacionFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"])) return null;
            var estado = rowData["Estado"]?.ToLower() == "activo" || rowData["Estado"]?.ToLower() == "true" || rowData["Estado"] == "1";
            return new ViewModels.RegimenAfiliacionItemViewModel { Codigo = rowData["Codigo"].ToUpper(), Nombre = rowData["Nombre"], Descripcion = rowData["Descripcion"], Estado = estado, FechaCreacion = DateTime.UtcNow };
        }

        private List<string> ValidateRegimenAfiliacion(ViewModels.RegimenAfiliacionItemViewModel item, int rowNumber) => new();
        #endregion
    }
}