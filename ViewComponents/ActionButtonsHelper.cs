using Highdmin.ViewComponents;

namespace Highdmin.ViewComponents
{
    public static class ActionButtonsHelper
    {
        public static ActionButtonsOptions CreateDefault(int recordId, string controller, string recordIdentifier = "")
        {
            return new ActionButtonsOptions
            {
                RecordId = recordId,
                Controller = controller,
                RecordIdentifier = recordIdentifier
            };
        }

        public static ActionButtonsOptions WithView(this ActionButtonsOptions options, bool show = true, string? customAction = null)
        {
            options.ViewButton = ActionButtonDefaults.ViewButton;
            options.ViewButton.Show = show;
            if (!string.IsNullOrEmpty(customAction))
                options.ViewButton.Action = customAction;
            return options;
        }

        public static ActionButtonsOptions WithEdit(this ActionButtonsOptions options, bool show = true, string? customAction = null)
        {
            options.EditButton = ActionButtonDefaults.EditButton;
            options.EditButton.Show = show;
            if (!string.IsNullOrEmpty(customAction))
                options.EditButton.Action = customAction;
            return options;
        }

        public static ActionButtonsOptions WithDelete(this ActionButtonsOptions options, bool show = true, string? customMessage = null)
        {
            options.DeleteButton = ActionButtonDefaults.DeleteButton;
            options.DeleteButton.Show = show;
            if (!string.IsNullOrEmpty(customMessage))
                options.DeleteButton.ConfirmationMessage = customMessage;
            return options;
        }

        public static ActionButtonsOptions WithCustomButton(this ActionButtonsOptions options, ActionButtonConfig customButton)
        {
            options.CustomButtons.Add(customButton);
            return options;
        }

        public static ActionButtonsOptions WithSize(this ActionButtonsOptions options, ActionButtonSize size)
        {
            options.Size = size;
            return options;
        }

        public static ActionButtonsOptions WithContainerClass(this ActionButtonsOptions options, string containerClass)
        {
            options.ContainerClass = containerClass;
            return options;
        }
    }
}