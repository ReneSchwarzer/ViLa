var config = 
{
	type: 'line',
	data: 
	{
		datasets: 
		[{
			label: 'Verlauf',
			backgroundColor: 'rgb(54, 162, 235)',
			borderColor: 'rgb(54, 162, 235)',
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
