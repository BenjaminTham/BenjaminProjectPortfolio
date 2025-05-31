var suTable;

function camelize(str) {
    return str.replace(/(?:^\w|[A-Z]|\b\w|\s+)/g, function (match, index) {
        if (+match === 0) return ""; // or if (/\s+/.test(match)) for white spaces
        return index === 0 ? match.toLowerCase() : match.toUpperCase();
    });
}


function toPascalCase(string) {
    return `${string}`
        .replace(new RegExp(/[-_]+/, 'g'), ' ')
        .replace(new RegExp(/[^\w\s]/, 'g'), '')
        .replace(
            new RegExp(/\s+(.)(\w*)/, 'g'),
            ($1, $2, $3) => `${$2.toUpperCase() + $3.toLowerCase()}`
        )
        .replace(new RegExp(/\w/), s => s.toUpperCase());
}

$(document).on('click', '[su-reset-form]', function () {
    $(this).closest('form').trigger("reset");


    //'hierarchy-select' reset
    var hierarchy = $(this).closest('form').find('.hierarchy-select');
    hierarchySelectReset(hierarchy);

    function hierarchySelectReset(t) {
        $(t).find('button').html($(t).find("a").first().html());
        $(t).find('button').siblings('input').val('');
        $(t).find("a").removeClass('active').removeAttr('data-default-selected');
        $(t).find("a").first().addClass('active').attr('data-default-selected', '');
    }

});


//$(function () {
//    var loaderText = document.getElementById("loading-msg");
//    var refreshIntervalId = setInterval(function () {
//        loaderText.innerHTML = getLoadingText();
//    }, 2000);

//    function getLoadingText() {
//        var strLoadingText;
//        var arrLoadingText = ["Action Executing", "Page Loading..."];
//        var rand = Math.floor(Math.random() * arrLoadingText.length);
//        return arrLoadingText[rand];
//    }
//})();

$(function () {
    console.log("loader called");
    $(".su-preloader").delay(400).fadeOut(400);
    $("body").css("overflow", "scroll");
});

var SuLoader = {
    start: function () { $(".su-preloader").fadeIn(400) },
    done: function () { $(".su-preloader").delay(400).fadeOut(400) }
};


function renderValidationError(errorArray) {
    $(".validationerror").empty();
    $(".validationerror").append("<ul></ul>");
    $.each(errorArray, function (index, value) {
        $(".validationerror ul").append("<li>" + value + "</li>");
    });
}

/* modal backdrop fix */
$(document).on('show.bs.modal', '.modal', function (event) {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});