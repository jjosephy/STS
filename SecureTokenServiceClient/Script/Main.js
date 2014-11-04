$(document).ready(function () {

    var qs = location.search;
    if (qs) {
        var p = qs.split('=');
        if (p[1] == "f") {
            $("#error").show();
            $("#error").html("Unauthenticated User");
            $("#error").css('color', 'red');
        }
    }
});