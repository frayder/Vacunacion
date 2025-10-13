/**
 * Helper para manejar modales de eliminación
 */
class DeleteModalHelper {
    constructor(options = {}) {
        this.modalId = options.modalId || 'deleteRecordModal';
        this.formId = options.formId || 'deleteForm';
        this.recordTextId = options.recordTextId || 'delete-record-text';
    }

    /**
     * Muestra el modal de eliminación
     * @param {string} deleteUrl - URL para el formulario de eliminación
     * @param {string} recordText - Texto descriptivo del registro a eliminar
     */
    show(deleteUrl, recordText = '') {
        const modal = document.getElementById(this.modalId);
        const form = document.getElementById(this.formId);
        const recordTextElement = document.getElementById(this.recordTextId);
        
        if (form) {
            form.action = deleteUrl;
        }
        
        if (recordTextElement && recordText) {
            recordTextElement.innerText = recordText;
        }
        
        if (modal) {
            const bootstrapModal = new bootstrap.Modal(modal);
            bootstrapModal.show();
        }
    }

    /**
     * Configurar el modal para un elemento específico
     * @param {string} actionUrl - URL base de la acción
     * @param {string} controller - Nombre del controlador
     */
    setupForController(actionUrl, controller) {
        this.actionUrl = actionUrl;
        this.controller = controller;
    }

    /**
     * Eliminar registro genérico
     * @param {number} id - ID del registro
     * @param {string} displayText - Texto a mostrar en el modal
     */
    confirmDelete(id, displayText) {
        const deleteUrl = `${this.actionUrl}/${id}`;
        this.show(deleteUrl, displayText);
    }
}

// Instancia global
window.deleteModalHelper = new DeleteModalHelper();