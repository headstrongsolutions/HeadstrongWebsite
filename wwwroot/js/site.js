function formatDateTime(datetimeString) {
    datetimeString.replace("T1","|");
    return moment(datetimeString, "YYYY-MM-DD|HH:mm:SS").toDate();
}

function nullCheckValue(value) {
    if(value == null){
        return "";
    }
    else{
        return value;
    }
}

function generateTemperature(last7DaysCollection) {
    var returnSet = new Array();
    returnSet.push(['Timestamp', 'Temperature']);
    var counter = 0;
    $(last7DaysCollection).each(function(index, value) {
        returnSet.push([formatDateTime(value.Received), nullCheckValue(parseFloat(value.Temperature))]);
    });

    return returnSet
}

function getLowestTemperature(last7DaysCollection) {
    var temperatureSet = new Array();
    var lowestSet = last7DaysCollection.reverse().slice(0,96);
    for (i = 0; i < lowestSet.length; i++) { 
        if(!isNaN(lowestSet[i].Temperature)){
            temperatureSet.push(lowestSet[i].Temperature);
        }
    }
    return Math.min.apply(null, temperatureSet);
}

function formatTemperatureData(last7DaysCollection) {
    if (last7DaysCollection.length > 0) {
        var temperatureData = new Array();
        temperatureData = generateTemperature(last7DaysCollection);
        var latesttemp = "<h2>Latest Temperature: " + last7DaysCollection[last7DaysCollection.length - 1].Temperature + "c</h2>";
        $("#latesttemp").html(latesttemp);
        var lowesttemp = "<h2>Lowest Temperature: " + getLowestTemperature(last7DaysCollection) + "c</h2>";
        $("#lowesttemp").html(lowesttemp);

        $.getScript('https://www.gstatic.com/charts/loader.js', function () {
            google.charts.load('current', { 'packages': ['corechart'] });
            google.charts.setOnLoadCallback(function () {
                var temperatureOptions = {
                    explorer: { axis: 'horizontal' },
                    hAxis: {
                        viewWindowMode: 'maximized'
                    },
                    tooltip: { isHtml: true },
                    title: 'Temperature',
                    curveType: 'function',
                    legend: { position: 'bottom' }
                };

                var chart = new google.visualization.LineChart(document.getElementById('curve_chart-temperature'));
                chart.draw(google.visualization.arrayToDataTable(temperatureData), temperatureOptions);
            });
        });
    }
    else {
        var returnMessage = "<p><b style=\"color:red;\">Data returned from server is empty (no lines in the last 7 days?)</b></p>";
        $(".panel").html(returnMessage)
    }
}

$(document).ready(function(){
   // Check if the temperatureChart html element is present
   if($("#curve_chart-temperature").length)
   {
        // LOAD DATA	
        var last7DaysDataRaw;
        $.getJSON( "/jarvis/gettempdata/garden")
        .done(function( json ) {
            formatTemperatureData(json.reverse())
        })
        .fail(function( jqxhr, textStatus, error ) {
            var err = textStatus + ", " + error;
            console.log( "Request Failed: " + err );
        });
   }
});