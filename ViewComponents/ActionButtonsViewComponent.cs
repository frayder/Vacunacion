using Microsoft.AspNetCore.Mvc;

namespace Highdmin.ViewComponents
{
    public class ActionButtonsViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(ActionButtonsOptions options)
        {
            return View(options);
        }
    }

    public class ActionButtonsOptions
    {
        public int RecordId { get; set; }
        public string? RecordIdentifier { get; set; } // Para mostrar en el modal de confirmación
        public string Controller { get; set; } = string.Empty;
        
        // Configuración de botones
        public ActionButtonConfig? ViewButton { get; set; }
        public ActionButtonConfig? EditButton { get; set; }
        public ActionButtonConfig? DeleteButton { get; set; }
        public List<ActionButtonConfig> CustomButtons { get; set; } = new List<ActionButtonConfig>();
        
        // Layout
        public string ContainerClass { get; set; } = "d-flex gap-2 mx-auto justify-content-center";
        public ActionButtonSize Size { get; set; } = ActionButtonSize.Small;
    }

    public class ActionButtonConfig
    {
        public bool Show { get; set; } = true;
        public string Text { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string CssClass { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string OnClick { get; set; } = string.Empty; // Para JavaScript personalizado
        public string Url { get; set; } = string.Empty; // Para URLs personalizadas
        public ActionButtonType Type { get; set; } = ActionButtonType.Link;
        public string? Tooltip { get; set; }
        public bool RequireConfirmation { get; set; } = false;
        public string? ConfirmationMessage { get; set; }
        public Dictionary<string, object> HtmlAttributes { get; set; } = new Dictionary<string, object>();
    }

    public enum ActionButtonType
    {
        Link,
        Button,
        Submit
    }

    public enum ActionButtonSize
    {
        ExtraSmall,
        Small,
        Medium,
        Large
    }

    public static class ActionButtonDefaults
    {
        public static ActionButtonConfig ViewButton => new ActionButtonConfig
        {
            Text = "Ver",
            Icon = "ri-eye-line",
            CssClass = "btn btn-info",
            Action = "Details",
            Type = ActionButtonType.Link
        };

        public static ActionButtonConfig EditButton => new ActionButtonConfig
        {
            Text = "",
            Icon = "ri-pencil-fill",
            CssClass = "btn btn-success",
            Action = "Edit",
            Type = ActionButtonType.Link
        };

        public static ActionButtonConfig DeleteButton => new ActionButtonConfig
        {
            Text = "",
            Icon = "ri-delete-bin-fill",
            CssClass = "btn btn-danger",
            Type = ActionButtonType.Button,
            RequireConfirmation = true,
            ConfirmationMessage = "¿Está seguro de que desea eliminar este registro?"
        };
    }
}