var vila = vila || {};

vila.ChartCtrl = class extends webexpress.webui.Ctrl {
	constructor(element) {
		super(element);

		this._intervalMs = parseInt(element.dataset.interval, 10) || 5000;
		this._timerId = null;

		this._start();
	}

	_start() {
		this._refresh();

		this._timerId = setInterval(() => {
			this._refresh();
		}, this._intervalMs);
	}

	_refresh() {
		$.ajax({ url: restUrl, dataType: 'json' }).then((data) => {
			this._updateChart(data);
		});
	}

	_updateChart(data) {
		if (typeof config_chart === 'undefined' || config_chart == null) {
			return;
		}

		if (typeof chart_chart === 'undefined' || chart_chart == null) {
			return;
		}

		config_chart.data.labels = data.ChartLabels;

		config_chart.data.datasets.forEach(function(dataset) {
			dataset.data = data.ChartData;
		});

		chart_chart.update();
	}
};

webexpress.webui.Controller.registerClass("wx-vila-chart", vila.ChartCtrl);

