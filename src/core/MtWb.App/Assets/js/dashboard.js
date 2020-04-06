$(document).ready(function() 
{
    setInterval(function () 
    {
        $.ajax({ url: "/api", dataType:'json' }).then(function(data) 
        {
           $('#now').html(data.Now);
           $('#measurementtime').html("Ladedauer: " + data.MeasurementTime);
           $('#power').html("Verbrauch: " + data.Power + " kWh");
           $('#cost').html("Angefallene Kosten: " + data.Cost + " €");
        });
    }, 1000);
});