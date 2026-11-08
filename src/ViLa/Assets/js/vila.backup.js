/**
 * Connects standard WebExpress upload events to the backup page alerts.
 */
(() => {
    /**
     * Displays the outcome of an upload originating from the backup drop area.
     * @param {CustomEvent} event - The WebExpress upload event.
     * @param {boolean} successful - Whether the server completed the import.
     * @returns {void}
     */
    function showResult(event, successful) {
        if (!event.detail?.sender?.classList.contains("vila-backup-upload")) {
            return;
        }
        const success = document.getElementById("backup_result_success");
        const error = document.getElementById("backup_result_error");
        const unavailable = document.getElementById("backup_result_unavailable");
        if (!success || !error || !unavailable) {
            return;
        }
        success.classList.toggle("d-none", !successful);
        const invalidBackup = !successful && event.detail.status === 400;
        error.classList.toggle("d-none", !invalidBackup);
        unavailable.classList.toggle("d-none", successful || invalidBackup);
    }

    // retain the native upload behavior and only display its server outcome
    document.addEventListener(webexpress.webui.Event.UPLOAD_SUCCESS_EVENT, event => showResult(event, true));
    document.addEventListener(webexpress.webui.Event.UPLOAD_ERROR_EVENT, event => showResult(event, false));
})();
