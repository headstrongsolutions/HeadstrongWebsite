//Site Functions 

function formatDateTime(datetimeString){
    datetimeString.replace("T1","|");
    return moment(datetimeString, "YYYY-MM-DD|HH:mm:SS").toDate();
  }

  function nullCheckValue(value){
    if(value == null){
      return "";
    }
    else{
      return value;
    }
  }

  function generateTemperature(data){
    var returnSet = new Array();
    returnSet.push(['Timestamp', 'Temperature']);
    var counter = 0;
    $(last7DaysCollection).each(function(index, value){
        returnSet.push([formatDateTime(value.Received), nullCheckValue(value.Temperature)]);
    });

    return returnSet
  }

  function getLowestTemperature(){
    var temperatureSet = new Array();
    var lowestSet = last7DaysCollection.reverse();
    for (i = 0; i < 96; i++) { 
        if(!isNaN(lowestSet[i].Temperature)){
            temperatureSet.push(lowestSet[i].Temperature);
        }
    }
    return Math.min.apply(null, temperatureSet);
  }

$(document).ready(function(){
   // Check if the temperatureChart html element is present
   if($("#curve_chart-temperature").length)
   {
    // LOAD DATA	
      var last7DaysDataRaw;
      $.getJSON( "http://garage:5002/externalcomms/GetLast7DaysTemperature/garden")
      .done(function( json ) {
        last7DaysDataRaw = json;
      })
      .fail(function( jqxhr, textStatus, error ) {
        var err = textStatus + ", " + error;
        console.log( "Request Failed: " + err );
    });
      
      var last7DaysCollection = last7DaysDataRaw.DHTPackages.reverse();  
      
      
      
    if(last7DaysCollection.length >0){
        var temperatureHouse = new Array();
        temperatureHouse = generateTemperature(last7DaysCollection); 
        var latesttemp = "<h2>Latest Temperature: " + last7DaysCollection[last7DaysCollection.length-1].Temperature + "c</h2>";
        $("#latesttemp").html(latesttemp);
        var lowesttemp = "<h2>Lowest Temperature: " + getLowestTemperature() + "c</h2>";
        $("#lowesttemp").html(lowesttemp);

        // GENERATE CHART
        google.charts.load('current', {'packages':['corechart']});
          google.charts.setOnLoadCallback(drawChart);
    
          function drawChart() {
            var houseTemperatureData = google.visualization.arrayToDataTable(temperatureHouse);
    
            var temperatureOptions = {
              explorer:{axis:'horizontal'},
              hAxis:{
                viewWindowMode:'maximized'
              },
              tooltip: {isHtml: true},
              title: 'Temperature',
              curveType: 'function',
              legend: { position: 'bottom' }
            };
    
            var chartTemperatureHouse = new google.visualization.LineChart(document.getElementById('curve_chart-temperature'));
            chartTemperatureHouse.draw(houseTemperatureData, temperatureOptions);
      }
    }  
    else{
        var returnMessage = "<p><b style=\"color:red;\">Data returned from server is empty (no lines in the last 7 days?)</b></p>";
        $(".panel").html(returnMessage)
    }
}
});