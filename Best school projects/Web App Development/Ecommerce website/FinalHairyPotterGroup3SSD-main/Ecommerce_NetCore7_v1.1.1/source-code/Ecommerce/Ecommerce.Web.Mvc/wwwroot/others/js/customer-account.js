$(function () {
    $('#customer_account_tab a').each(function () {
        let hrefUrl = $(this).attr("href");
        let pathUrl = window.location.pathname;
        if (hrefUrl.toLowerCase() == pathUrl.toLowerCase()) {
            $(this).addClass('active');
        }
    });
})