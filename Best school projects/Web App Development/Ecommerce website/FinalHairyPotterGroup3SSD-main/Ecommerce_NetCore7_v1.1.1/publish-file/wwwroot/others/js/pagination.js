//Sort-Item Active
$('tr .item-sort, .sort-section .item-sort').each(function () {
    var hrefurl = $(this).attr("href");
    var pathurl = window.location.search;
    var hrefsortColumn = getURLParameter(hrefurl, 'sortColumn');
    var pathsortColumn = getURLParameter(pathurl, 'sortColumn');

    if (hrefsortColumn != null && pathsortColumn !== null) {
        if (hrefsortColumn.toLowerCase() == pathsortColumn.toLowerCase()) {
            $(this).addClass('active');
            //$(this).parentsUntil(".nav-sidebar > .nav-treeview").addClass('menu-open').prev('a').addClass('active');
        }
    }

});

function getURLParameter(url, name) {
    return (RegExp(name + '=' + '(.+?)(&|$)').exec(url) || [, null])[1];
}

$(document).on('click', '.page-size-section .page-size-value', function (e) {
    $(this).toggleClass("show");
    var dropdownItem = e.target;
    var container = $(this).find(".selectBox__value");
    container.text(dropdownItem.text);
    $(dropdownItem)
        .addClass("active")
        .siblings()
        .removeClass("active");
});

//$(".page-size-section .page-size-value").on("click", function (e) {
//    $(this).toggleClass("show");
//    var dropdownItem = e.target;
//    var container = $(this).find(".selectBox__value");
//    container.text(dropdownItem.text);
//    $(dropdownItem)
//        .addClass("active")
//        .siblings()
//        .removeClass("active");
//});


//$('.page-size-section .page-size-value .dropdown-menu a').each(function () {
//    var hrefurl = $(this).attr("href");
//    var pathurl = window.location.search;
//    var hrefsortColumn = getURLParameter(hrefurl, 'length');
//    var pathsortColumn = getURLParameter(pathurl, 'length');

//    if (hrefsortColumn != null && pathsortColumn !== null) {
//        if (hrefsortColumn.toLowerCase() == pathsortColumn.toLowerCase()) {
//            $(this).addClass('active');
//            $('.selectBox__value').html($('.dropdown-item.active').text());
//        }
//    }
//});