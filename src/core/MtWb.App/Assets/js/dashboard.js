var config = 
{
	type: 'line',
	data: 
	{
		labels: [],
		datasets: 
		[{
			label: 'Verlauf',
			backgroundColor: 'rgb(54, 162, 235)',
			borderColor: 'rgb(54, 162, 235)',
			data: [],
			fill: false,
		}]
	},
	options: 
	{
		responsive: true,
		title: 
		{
			display: false,
			text: ''
		},
		tooltips: 
		{
			mode: 'index',
			intersect: false,
		},
		hover: 
		{
			mode: 'nearest',
			intersect: true
		},
		scales: 
		{
			x: 
			{
				display: true,
				scaleLabel: 
				{
					display: true,
					labelString: 'Zeit in Minuten'
				}
			},
			y: 
			{
				display: true,
				min: 0,
				suggestedMin: 0,
				scaleLabel: 
				{
					display: true,
					labelString: 'Verbrauch in kWh'
				}
			}
		}
	}
};

window.onload = function() 
{
    var ctx = document.getElementById('canvas').getContext('2d');
    window.myLine = new Chart(ctx, config);
};

$(document).ready(function() 
{
    setInterval(function () 
    {
        $.ajax({ url: "api", dataType:'json' }).then(function(data) 
        {
            $('#now').html(data.Now);
            $('#measurementtime').html("Ladedauer: " + data.MeasurementTime);
            $('#power').html("Verbrauch: " + data.Power + " kWh");
            $('#cost').html("Angefallene Kosten: " + data.Cost + " €");

			config.data.labels = data.ChartLabels;

		    config.data.datasets.forEach(function(dataset) 
		    {
				dataset.data = data.ChartData;
		    });

			window.myLine.update();
        });
    }, 1000);
});

