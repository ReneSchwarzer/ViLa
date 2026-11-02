/**
 * Displays the card tag badges while retaining the history table's inline editor.
 */
class ViLaHistoryTagEditor extends webexpress.webui.SmartEditCtrl {
    /**
     * Builds the badge list for the current measurement labels.
     * @param {string|string[]} value - The current editor value.
     * @returns {HTMLElement} The tag list or the standard empty editor.
     */
    _getView(value) {
        if (!value || value.length === 0) {
            return super._getView(value);
        }
        const list = document.createElement("div");
        list.className = "d-flex flex-wrap gap-1";
        for (const label of String(value).split(/[ ;]+/).filter(Boolean)) {
            const original = this._element.vilaTags.find(tag => tag.label.toLowerCase() === label.toLowerCase());
            const badge = document.createElement("span");
            badge.className = "badge vila-measurement-tag";
            badge.textContent = original?.label ?? label;
            if (original) {
                badge.style.setProperty("--vila-tag-color", original.color);
            }
            list.append(badge);
        }
        return list;
    }
}

/**
 * Creates an editable tag cell with the colors supplied by the measurement model.
 * @param {Object[]} tags - The labels and their display colors.
 * @param {Object} table - The table controller.
 * @param {Object} row - The measurement row and save endpoint.
 * @param {Object} cell - The source cell.
 * @param {string} name - The editable field name.
 * @param {Object} options - The column template options.
 * @returns {HTMLElement} The inline editor host.
 */
function renderViLaHistoryTags(tags, table, row, cell, name, options) {
    const host = document.createElement("div");
    host.id = row.id + "_" + name;
    host.vilaTags = tags;
    const field = document.createElement("div");
    field.setAttribute("name", name);
    field.setAttribute("data-placeholder", options.placeholder || "");
    const editor = new webexpress.webui.InputTagCtrl(field);
    field._wx_controller = editor;
    editor.value = tags.map(tag => tag.label).join(";");
    host.append(field);
    webexpress.webui.TableTemplates.bindInlineEdit(host, row, name);
    new ViLaHistoryTagEditor(host);
    return host;
}

webexpress.webui.TableTemplates.register("vila-tags", renderViLaHistoryTags);
