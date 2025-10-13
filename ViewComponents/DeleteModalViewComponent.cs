using Microsoft.AspNetCore.Mvc;

namespace Highdmin.ViewComponents
{
    public class DeleteModalViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(DeleteModalOptions options)
        {
            return View(options);
        }
    }

    public class DeleteModalOptions
    {
        public string ModalId { get; set; } = "deleteRecordModal";
        public string Title { get; set; } = "¿Está seguro?";
        public string Message { get; set; } = "¿Está seguro de que desea eliminar este registro?";
        public string ConfirmButtonText { get; set; } = "Sí, Eliminar";
        public string CancelButtonText { get; set; } = "Cancelar";
        public string ConfirmButtonClass { get; set; } = "btn w-sm btn-danger";
        public string CancelButtonClass { get; set; } = "btn w-sm btn-light";
        public string FormId { get; set; } = "deleteForm";
        public string RecordTextId { get; set; } = "delete-record-text";
        public string IconUrl { get; set; } = "https://cdn.lordicon.com/gsqxdxog.json";
        public string IconColors { get; set; } = "primary:#f7b84b,secondary:#f06548";
        public bool ShowRecordText { get; set; } = true;
    }
}