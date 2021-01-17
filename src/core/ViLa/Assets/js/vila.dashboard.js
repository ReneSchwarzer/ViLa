$(document).ready(function() 
{
    setInterval(function () 
    {
        $.ajax({ url: restUrl, dataType:'json' }).then(function(data) 
        {
            $('#now').html(data.Now);
            $('#measurementtime').html(vila_charging_duration.replace('{0}', data.MeasurementTime));
            $('#power').html(vila_charging_consumption.replace('{0:F2}', data.Power));
            $('#cost').html(vila_charging_cost.replace('{0:F2}', data.Cost).replace('{1}', currency));
			$('#current_name').html(data.CurrentPower + " kWh");

			if (data.ActiveCharging)
			{
				$('#submit_charging').removeClass('btn-success');
				$('#submit_charging').removeClass('btn-danger');
                $('#submit_charging').addClass('btn-danger');
				$('#submit_charging').html("&nbsp;&nbsp;&nbsp;" + vila_charging_stop);
				var span = document.createElement('span');
				span.setAttribute('class', 'fas fa-power-off');
			}
			else
			{
				$('#submit_charging').removeClass('btn-success');
				$('#submit_charging').removeClass('btn-danger');
                $('#submit_charging').addClass('btn-success');
				$('#submit_charging').html("&nbsp;&nbsp;&nbsp;" + vila_charging_begin);
				var span = document.createElement('span');
				span.setAttribute('class', 'fas fa-play-circle');
			}

			if  (config_chart != null)
			{
				config_chart.data.labels = data.ChartLabels;

				config_chart.data.datasets.forEach(function(dataset) 
				{
					dataset.data = data.ChartData;
				});

				chart_chart.update();
			}
        });
    }, 5000);
});

