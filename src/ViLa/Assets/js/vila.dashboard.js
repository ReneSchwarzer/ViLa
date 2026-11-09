var vila = vila || {};

/**
 * Refreshes live charging metrics through the WebExpress chart controller.
 */
vila.ChartCtrl = class extends webexpress.webui.Ctrl {
    /**
     * Initializes polling for a dashboard host.
     * @param {HTMLElement} element - The dashboard host with its endpoint and localized labels.
     */
    constructor(element) {
        super(element);
        this._host = element;
        this._timerId = null;
        this._request = null;
        this._disposed = false;
        this._refresh();
    }

    /**
     * Loads one snapshot and schedules the next request after completion.
     * @returns {Promise<void>} Resolves after the request has been processed.
     */
    async _refresh() {
        if (this._disposed) {
            return;
        }
        if (!this._host.isConnected || document.hidden) {
            this._timerId = setTimeout(() => this._refresh(), 5000);
            return;
        }
        this._request = new AbortController();
        const timeout = setTimeout(() => this._request?.abort(), 10000);
        try {
            if (!this._host.dataset.url) {
                throw new Error("The charging endpoint is missing.");
            }
            const response = await fetch(this._host.dataset.url, {
                cache: "no-store",
                headers: { "Accept": "application/json" },
                signal: this._request.signal
            });
            if (!response.ok) {
                throw new Error("The charging request failed.");
            }
            const data = await response.json();
            if (!Array.isArray(data.ChartLabels) || !Array.isArray(data.ChartData)) {
                throw new Error("The charging response is invalid.");
            }
            if (!this._disposed) {
                this._update(data);
                this._setText("vila-connection", this._host.dataset.connected);
                this._host.classList.remove("vila-offline");
            }
        } catch (error) {
            if (!this._disposed) {
                this._setText("vila-connection", this._host.dataset.offline);
                this._host.classList.add("vila-offline");
            }
        } finally {
            clearTimeout(timeout);
            this._request = null;
            if (!this._disposed) {
                this._timerId = setTimeout(() => this._refresh(), 5000);
            }
        }
    }

    /**
     * Applies a snapshot without replacing the chart or its surrounding content.
     * @param {Object} data - The charging API payload.
     * @returns {void}
     */
    _update(data) {
        this._setText("vila-current", data.CurrentPower + " kW");
        this._setText("vila-energy", data.Power + " kWh");
        this._setText("vila-cost", data.Cost + " " + this._host.dataset.currency);
        this._setText("vila-duration", data.MeasurementTime);
        this._setText("vila-status", data.ActiveCharging ? this._host.dataset.active : this._host.dataset.idle);
        this._host.classList.toggle("vila-charging", data.ActiveCharging);
        if (!data.ActiveCharging) {
            return;
        }
        const chartElement = this._host.querySelector("#vila-power-chart");
        const chart = chartElement && webexpress.webui.Controller.getInstanceByElement(chartElement);
        if (chart) {
            chart.updateData({
                labels: data.ChartLabels,
                datasets: [{
                    label: "kW",
                    data: data.ChartData.map(Number),
                    borderColor: "#168b80",
                    backgroundColor: "rgba(22, 139, 128, 0.12)",
                    fill: true,
                    pointRadius: 0,
                    tension: 0.2
                }]
            });
        }
    }

    /**
     * Updates a metric as plain text to preserve safe rendering of configured values.
     * @param {string} id - The metric element identifier.
     * @param {string} value - The formatted value.
     * @returns {void}
     */
    _setText(id, value) {
        const element = this._host.querySelector("#" + id);
        if (element) {
            element.textContent = value;
        }
    }

    /**
     * Cancels pending requests and timers when the dashboard is removed.
     * @returns {void}
     */
    destroy() {
        this._disposed = true;
        clearTimeout(this._timerId);
        this._request?.abort();
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("wx-vila-chart", vila.ChartCtrl);

/**
 * Persists assignment moves through DataKanban with an expected source column.
 */
vila.AssignmentBoardCtrl = class extends webexpress.webapp.KanbanCtrl {
    /**
     * Captures the source column before the standard persistence listener runs.
     * @param {HTMLElement} element - The data board element.
     * @returns {void}
     */
    _initRestPersistence(element) {
        this._onAssignmentMove = event => {
            this._assignmentSource = event.detail?.oldColumnId;
        };
        element.addEventListener(webexpress.webui.Event.MOVE_EVENT, this._onAssignmentMove, true);
        super._initRestPersistence(element);
    }

    /**
     * Adds the expected source to the standard asynchronous move request.
     * @param {Object} payload - The card destination supplied by DataKanban.
     * @returns {void}
     */
    _sendStateToServer(payload) {
        super._sendStateToServer({ ...payload, source: this._assignmentSource });
    }

    /**
     * Removes the source listener and releases the data board resources.
     * @returns {void}
     */
    destroy() {
        this._element.removeEventListener(webexpress.webui.Event.MOVE_EVENT, this._onAssignmentMove, true);
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("wx-vila-data-kanban", vila.AssignmentBoardCtrl);

/**
 * Applies a submitted WQL expression to the current page.
 */
vila.WqlSearchCtrl = class extends webexpress.webui.Ctrl {
    /**
     * The server-rendered history panes, as named by their "history-{view}-results" ids.
     */
    static VIEWS = ["list", "cards", "assignment"];

    /**
     * Restores the query and connects the history search control to the page.
     * @param {HTMLElement} element - The wrapper containing the search control.
     */
    constructor(element) {
        super(element);
        this._host = element;
        this._pending = {};
        const search = webexpress.webui.Controller.getInstanceByElement(element.firstElementChild);
        const params = new URL(window.location.href).searchParams;
        const query = params.get("q") || "";
        const basicQuery = params.get("search") || "";
        if (query) {
            search._wqlCtrl.value = query;
            search._applyMode("wql");
        } else if (basicQuery) {
            search._basicCtrl.value = basicQuery;
            search._applyMode("basic");
        }
        if (element.hasAttribute("data-wx-source-search")) {
            const initialPage = Number(params.get("page"));
            if (query || basicQuery || initialPage > 1) {
                requestAnimationFrame(() => {
                    const table = webexpress.webui.Controller.getInstanceByElement(document.getElementById("history-table"));
                    if (query || basicQuery) {
                        table?.search(query || basicQuery, query ? "wql" : "basic");
                    }
                    if (Number.isSafeInteger(initialPage) && initialPage > 1) {
                        table?.paging(initialPage - 1);
                    }
                });
            }
            // a pane left stale by an earlier search is refreshed once its view is opened
            this._onVisible = event => {
                if (!event.detail?.visible || !this._stale?.size) {
                    return;
                }
                const view = vila.WqlSearchCtrl.VIEWS.find(name => this._stale.has(name) && vila.WqlSearchCtrl._isVisible(name));
                if (view) {
                    this._refresh(view);
                }
            };
            document.addEventListener(webexpress.webui.Event.CHANGE_VISIBILITY_EVENT, this._onVisible);
            return;
        }
        this._onSearch = event => {
            if (event.detail?._fromAdvanced) {
                return;
            }
            const value = event.detail?.value || "";
            const basic = event.detail?.searchType === "basic";
            if (basic ? value === basicQuery && !query : value === query && !basicQuery) {
                return;
            }
            const url = new URL(window.location.href);
            url.searchParams.delete("page");
            url.searchParams.delete("vehicle");
            url.searchParams.delete(basic ? "q" : "search");
            url.searchParams.set(basic ? "search" : "q", value);
            window.location.assign(url);
        };
        element.addEventListener(webexpress.webui.Event.CHANGE_FILTER_EVENT, this._onSearch);
    }

    /**
     * Updates the server-rendered history panes for a bound search.
     * @param {string} value - The active filter text.
     * @param {string} mode - The basic or WQL search mode.
     * @returns {Promise<void>} Resolves after the current results are installed.
     */
    async search(value, mode) {
        const url = new URL(window.location.href);
        url.searchParams.delete("page");
        url.searchParams.delete("search");
        url.searchParams.delete("q");
        if (value) {
            url.searchParams.set(mode === "wql" ? "q" : "search", value);
        }
        window.history.replaceState(window.history.state, "", url);
        // only the visible pane is rebuilt now; the hidden ones are marked stale and
        // refreshed when they are shown, so the server never builds views nobody sees
        Object.values(this._pending).forEach(pending => pending.abort());
        this._stale = new Set(vila.WqlSearchCtrl.VIEWS);
        const visible = vila.WqlSearchCtrl.VIEWS.find(view => vila.WqlSearchCtrl._isVisible(view));
        if (visible) {
            await this._refresh(visible);
        }
    }

    /**
     * Applies the bound footer page to the server-rendered history views.
     * @param {number} index - The zero-based page selected in the footer.
     * @returns {Promise<void>} Resolves when the visible pane has been refreshed.
     */
    async paging(index) {
        const url = new URL(window.location.href);
        url.searchParams.set("page", String(index + 1));
        window.history.replaceState(window.history.state, "", url);
        this._stale ??= new Set();
        for (const view of vila.WqlSearchCtrl.VIEWS) {
            this._pending[view]?.abort();
            this._stale.add(view);
        }
        const visible = vila.WqlSearchCtrl.VIEWS.find(view => vila.WqlSearchCtrl._isVisible(view));
        if (visible) {
            await this._refresh(visible);
        }
    }

    /**
     * Replaces one server-rendered history pane with the results for the current address.
     * @param {string} view - The view name (list, cards or assignment).
     * @returns {Promise<void>} Resolves after the pane is installed.
     */
    async _refresh(view) {
        this._stale?.delete(view);
        const url = new URL(window.location.href);
        url.searchParams.set("view", view);
        this._pending[view]?.abort();
        const pending = new AbortController();
        this._pending[view] = pending;
        try {
            const response = await fetch(url, { signal: pending.signal });
            if (!response.ok) {
                throw new Error(String(response.status));
            }
            const page = new DOMParser().parseFromString(await response.text(), "text/html");
            const id = "history-" + view + "-results";
            const content = page.getElementById(id);
            if (content && !pending.signal.aborted) {
                document.getElementById(id)?.replaceChildren(...content.childNodes);
            }
        } catch (error) {
            if (!pending.signal.aborted) {
                url.searchParams.delete("view");
                window.location.assign(url);
            }
        }
    }

    /**
     * Determines whether a history pane is currently shown.
     * @param {string} view - The view name.
     * @returns {boolean} Whether the pane is in the visible view container.
     */
    static _isVisible(view) {
        const pane = document.getElementById("history-" + view + "-results");
        return !!pane && !pane.closest(".d-none");
    }

    /**
     * Removes the query listener when the search leaves the page.
     * @returns {void}
     */
    destroy() {
        Object.values(this._pending).forEach(pending => pending.abort());
        if (this._onVisible) {
            document.removeEventListener(webexpress.webui.Event.CHANGE_VISIBILITY_EVENT, this._onVisible);
        }
        if (this._onSearch) {
            this._host.removeEventListener(webexpress.webui.Event.CHANGE_FILTER_EVENT, this._onSearch);
        }
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("vila-wql-search", vila.WqlSearchCtrl);

/**
 * Refreshes the catalog after its REST editor saves and opens bookmarked dialogs.
 */
vila.VehicleCatalogCtrl = class extends webexpress.webui.Ctrl {
    /**
     * Connects the catalog table and its create, edit, and delete dialogs.
     * @param {HTMLElement} element - The catalog containing the REST controls.
     */
    constructor(element) {
        super(element);
        this._element = element;
        const controller = node => webexpress.webui.Controller.getInstanceByElement(node);
        const dialogs = [element.querySelector("#vehicle-create"), element.querySelector("#vehicle-edit"), element.querySelector("#vehicle-delete")];
        this._onSaved = event => {
            const sender = event.detail?.sender || event.detail?.form || event.target;
            const dialog = sender?.closest?.("#vehicle-create, #vehicle-edit, #vehicle-delete");
            if (dialog && dialogs.includes(dialog)) {
                controller(element.querySelector("#vehicle_catalog"))?.load();
            }
        };
        document.addEventListener(webexpress.webui.Event.SAVED_EVENT, this._onSaved, true);
        const params = new URL(window.location.href).searchParams;
        const table = controller(element.querySelector("#vehicle_catalog"));
        const search = controller(element.querySelector("#vehicle-search"));
        const wql = params.get("q");
        const name = params.get("search");
        if (wql) {
            search._wqlCtrl.value = wql;
            search._applyMode("wql");
        } else if (name) {
            search._basicCtrl.value = name;
            search._applyMode("basic");
        }
        const page = Number(params.get("page"));
        if (Number.isSafeInteger(page) && page > 1) {
            table.paging(page - 1);
        }
        const selected = params.get("vehicle");
        if (selected) {
            controller(element.querySelector(selected === "new" ? "#vehicle-create" : "#vehicle-edit")).show();
        }
    }

    /**
     * Removes the save listener when the catalog is disposed.
     * @returns {void}
     */
    destroy() {
        document.removeEventListener(webexpress.webui.Event.SAVED_EVENT, this._onSaved, true);
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("vila-vehicle-catalog", vila.VehicleCatalogCtrl);

/**
 * Updates a measurement's reading view when its editor confirms a save.
 */
vila.DescriptionCtrl = class extends webexpress.webui.Ctrl {
    /**
     * Connects the editor to the reading view in the same detail panel.
     * @param {HTMLElement} element - The panel containing the content and editor controls.
     */
    constructor(element) {
        super(element);
        this._onSaved = event => {
            const body = event.detail?.data?.Body;
            if (typeof body === "string") {
                const content = webexpress.webui.Controller.getInstanceByElement(element.firstElementChild);
                content.value = body;
                const dialog = element.querySelector("dialog[open]");
                if (dialog) {
                    webexpress.webui.Controller.getInstanceByElement(dialog).hide();
                }
            }
        };
        element.addEventListener(webexpress.webui.Event.SAVED_EVENT, this._onSaved);
    }

    /**
     * Removes the listener when the detail panel is replaced.
     * @returns {void}
     */
    destroy() {
        this._element.removeEventListener(webexpress.webui.Event.SAVED_EVENT, this._onSaved);
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("vila-description", vila.DescriptionCtrl);
