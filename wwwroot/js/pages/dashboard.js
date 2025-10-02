/**
 * Theme: Highdmin - Responsive Bootstrap 5 Admin Dashboard
 * Author: I Analitycs
 * Module/App: Dashboard
 */

//
// Statistics CHART
//
///
var colors = ["#777edd", "#0acf97", "#fa5c7c", "#f9bc0b"];
var dataColors = $("#statistics-chart").data('colors');
if (dataColors) {
    colors = dataColors.split(",");
}

var options = {
    series: [
        { name: "Open Campaign", type: "bar", data: [89.25, 98.58, 68.74, 108.87, 77.54, 84.03, 51.24, 28.57, 92.57, 42.36, 88.51, 36.57] },
        { name: "Marketing Cost", type: "bar", data: [30.28, 33.45, 50.0, 31.12, 26.59, 34.06, 39.79, 14.38, 33.44, 48.12, 27.91, 23.91] },
    ],
    chart: { height: 301, type: "line", toolbar: { show: false } },
    stroke: {
        width: 0,
        curve: 'straight'
    },
    plotOptions: {
        bar: {
            columnWidth: "45%",
            barHeight: "70%",
            borderRadius: 5,
        },
    },
    xaxis: { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"] },
    colors: colors
}

var chart = new ApexCharts(
    document.querySelector("#statistics-chart"),
    options
);

chart.render();

//
// REVENUE AREA CHART
//
///
var colors = ["#777edd", "#0acf97", "#fa5c7c", "#f9bc0b"];
var dataColors = $("#revenue-chart").data('colors');
if (dataColors) {
    colors = dataColors.split(",");
}

var options = {
    series: [
        { 
            name: "Total Income", 
            data: [82.0, 85.0, 70.0, 90.0, 75.0, 78.0, 65.0, 50.0, 72.0, 60.0, 80.0, 70.0] 
        },
        { 
            name: "Total Expenses", 
            data: [30.0, 32.0, 40.0, 35.0, 30.0, 36.0, 37.0, 28.0, 34.0, 42.0, 38.0, 30.0] 
        },
    ],
    stroke: {
        width: 2,
        curve: 'straight'
    },
    chart: {
        height: 299,
        type: 'area',
        zoom: {
            enabled: false
        },
        toolbar: { show: false }
    },
    dataLabels: {
        enabled: false
    },
    xaxis: { categories: ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"] },
    colors: colors,
    tooltip: {
        shared: true,
        y: [{
            formatter: function (y) {
                if (typeof y !== "undefined") {
                    return "$" + y.toFixed(2) + "k";
                }
                return y;
            },
        },
        {
            formatter: function (y) {
                if (typeof y !== "undefined") {
                    return "$" + y.toFixed(2) + "k";
                }
                return y;
            },
        }
        ],
    },
}

var chart = new ApexCharts(
    document.querySelector("#revenue-chart"),
    options
);

chart.render();

//
// data-visits- CHART
//
var colors = ["#777edd", "#45bbe0", "#0acf97", "#fa5c7c", "#e3eaef"];
var dataColors = $("#data-visits-chart").data('colors');
if (dataColors) {
    colors = dataColors.split(",");
}

var options = {
    chart: {
        height: 335,
        type: 'donut',
    },
    series: [25, 40, 30, 15, 20], // Example age group data
    legend: {
        show: true,
        position: 'bottom',
        horizontalAlign: 'center',
        verticalAlign: 'middle',
        floating: false,
        fontSize: '14px',
        offsetX: 0,
        offsetY: 7
    },
    labels: ["10-16 (Child)", "18-26 (Young)", "27-35 (Adult)", "36-50 (Middle Age)", "51+ (Senior)"], // Age groups
    colors: colors,
    responsive: [{
        breakpoint: 600,
        options: {
            chart: {
                height: 240
            },
            legend: {
                show: false
            },
        }
    }]
};

var chart = new ApexCharts(
    document.querySelector("#data-visits-chart"),
    options
);

chart.render();