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
            // Aquí se pueden agregar más configuraciones
            // ConfigurePaciente();
            // ConfigureAseguradora();
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

        private void ConfigureTipoCarnet()
        {
            // Configuración para importar TipoCarnetItemViewModel
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
                    new Dictionary<string, string>
                    {
                        { "Codigo", "CC" },
                        { "Nombre", "Cédula de Ciudadanía" },
                        { "Descripcion", "Documento de identificación para ciudadanos colombianos" },
                        { "Estado", "Activo" }
                    },
                    new Dictionary<string, string>
                    {
                        { "Codigo", "TI" },
                        { "Nombre", "Tarjeta de Identidad" },
                        { "Descripcion", "Documento de identificación para menores de edad" },
                        { "Estado", "Activo" }
                    }
                },
                MapFunction = MapTipoCarnetFromRow,
                CustomValidation = ValidateTipoCarnet
            };

            // Configuración para exportar TipoCarnet (modelo de base de datos)
            var exportConfig = new ExportConfiguration<Models.TipoCarnet>
            {
                SheetName = "Tipos de Carnet",
                FileName = "TiposCarnet",
                ColumnMappings = new Dictionary<string, string>
                {
                    { "Codigo", "Código" },
                    { "Nombre", "Nombre" },
                    { "Descripcion", "Descripción" },
                    { "Estado", "Estado" },
                    { "FechaCreacion", "Fecha Creación" }
                },
                CustomMapping = item => new Dictionary<string, object>
                {
                    { "Codigo", item.Codigo },
                    { "Nombre", item.Nombre },
                    { "Descripcion", item.Descripcion ?? "" },
                    { "Estado", item.Estado ? "Activo" : "Inactivo" },
                    { "FechaCreacion", item.FechaCreacion.ToString("dd/MM/yyyy") }
                }
            };

            _importConfigurations[typeof(ViewModels.TipoCarnetItemViewModel)] = importConfig;
            _exportConfigurations[typeof(Models.TipoCarnet)] = exportConfig;
        }

        private ViewModels.TipoCarnetItemViewModel? MapTipoCarnetFromRow(Dictionary<string, string> rowData)
        {
            if (string.IsNullOrEmpty(rowData["Codigo"]) || string.IsNullOrEmpty(rowData["Nombre"]))
                return null;

            var estadoTexto = rowData["Estado"]?.Trim();
            var estado = estadoTexto?.ToLower() == "activo" || estadoTexto?.ToLower() == "true" || estadoTexto == "1";

            return new ViewModels.TipoCarnetItemViewModel
            {
                Codigo = rowData["Codigo"].ToUpper(),
                Nombre = rowData["Nombre"],
                Descripcion = rowData["Descripcion"],
                Estado = estado,
                FechaCreacion = DateTime.Now
            };
        }

        private List<string> ValidateTipoCarnet(ViewModels.TipoCarnetItemViewModel item, int rowNumber)
        {
            var errors = new List<string>();
            
            // Validaciones personalizadas específicas para TipoCarnet
            if (item.Codigo.Length > 10)
            {
                errors.Add($"Fila {rowNumber}: El código no puede tener más de 10 caracteres");
            }

            if (item.Nombre.Length > 255)
            {
                errors.Add($"Fila {rowNumber}: El nombre no puede tener más de 255 caracteres");
            }

            if (!string.IsNullOrEmpty(item.Descripcion) && item.Descripcion.Length > 500)
            {
                errors.Add($"Fila {rowNumber}: La descripción no puede tener más de 500 caracteres");
            }

            return errors;
        }
    }
}