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
        this._themeObserver = new MutationObserver(() => this._applyTheme());
        this._themeObserver.observe(document.documentElement, {
            attributes: true,
            subtree: true,
            attributeFilter: ["data-wx-theme"]
        });
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
                this._applyTheme();
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
     * Applies the current WebExpress theme to both dashboard chart axes and legends.
     * @returns {void}
     */
    _applyTheme() {
        for (const id of ["vila-power-chart", "vila-monthly-chart"]) {
            const element = document.getElementById(id);
            const chart = element && webexpress.webui.Controller.getInstanceByElement(element);
            if (!chart) {
                continue;
            }
            const style = getComputedStyle(element);
            const color = style.color;
            const grid = style.getPropertyValue("--wx-border-color").trim() || "#dee2e6";
            chart.options({
                color,
                plugins: { legend: { labels: { color } } },
                scales: {
                    x: { ticks: { color }, title: { color }, grid: { color: grid } },
                    y: { ticks: { color }, title: { color }, grid: { color: grid } }
                }
            });
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
        this._themeObserver.disconnect();
        super.destroy();
    }
};

webexpress.webui.Controller.registerClass("wx-vila-chart", vila.ChartCtrl);
