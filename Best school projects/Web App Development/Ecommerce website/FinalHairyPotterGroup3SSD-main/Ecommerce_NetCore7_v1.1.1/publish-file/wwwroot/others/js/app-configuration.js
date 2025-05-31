$(function () {
    //AppConfiguration Nav Active
    //$('#appconfig-tab li a').each(function () {
    //    //$(this).removeClass('active');
    //    var hrefurl = $(this).attr("href");
    //    var pathurl = window.location.pathname;
    //    if (hrefurl.toLowerCase() == pathurl.toLowerCase()) {
    //        $(this).addClass('active');
    //    }
    //});

    $('#appconfig-tab a').each(function () {
        //$(this).removeClass('active');
        var hrefurl = $(this).attr("href");
        var pathurl = window.location.pathname;
        if (hrefurl.toLowerCase() == pathurl.toLowerCase()) {
            $(this).addClass('active');
        }
    });

})