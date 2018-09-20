$(window).on('beforeunload', function () {

    $("#spinnerProcessing").fadeIn();
    animateSpinner();

});

function animateSpinner() {
    var opts = {
        lines: 12, // The number of lines to draw
        length: 7, // The length of each line
        width: 4, // The line thickness
        radius: 10, // The radius of the inner circle
        color: '#006699', // ##rgb or ##rrggbb
        scale: 3,
        rotate: 0,
        animation: 'spinner-line-fade-default',
        direction: 1, // 1: clockwise, -1: counterclockwise
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        speed: 2, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false // Whether to use hardware acceleration
    };

    var target = document.getElementById('spinnerProcessing');
    var spinner = new Spinner(opts).spin(target);
}