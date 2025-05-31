
window.addEventListener('DOMContentLoaded', event => {

    // Toggle the side navigation
    var sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (sidebarToggle) {
        // Uncomment Below to persist sidebar toggle between refreshes
        // if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
        //     document.body.classList.toggle('sb-sidenav-toggled');
        // }
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }

});


//$(function () {
//    //datepicker
//    flatpickr($("input[type=date]"), {
//        //defaultDate: new Date(),
//        //dateFormat: "DD-MM-YYYY"
//    });
//});


//$(document).ready(function () {
//    $('select').niceSelect();
//});

$(function () {
    //Datatable Default setting
    $.extend($.fn.dataTable.defaults, {
        "language": {
            "paginate": {
                "previous": "<i class='fas fa-angle-left'></i>",
                "next": "<i class='fas fa-angle-right'></i>",
            },
            zeroRecords: "No matching records found"
        },
        "drawCallback": function (settings) {
            fotmatPaging();
            //RenderAction();
            //console.log(settings.json);
        },
        //"initComplete": function (settings, json) {
        //    RenderAction();
        //},
        "pagingType": "full_numbers",
    });

    //Datatable page format(hide extra page-number)
    function fotmatPaging() {
        $('.dataTables_paginate .pagination .paginate_button').each(function () {
            if ($(this).hasClass("active") || $(this).prev().hasClass("active") || $(this).next().hasClass("active") || $(this).hasClass("first") || $(this).hasClass("last")) {

            } else {
                $(this).css("display", "none");
            }

            if ($(this).hasClass("previous") || $(this).hasClass("next")) {
                $(this).css("display", "none");
            }
        });
    }


    //Datatable Server Side ajax Call
    ////////////////////////////////////////
    // Pipelining function for DataTables. To be used to the `ajax` option of DataTables
    // Copied from https://datatables.net/examples/server_side/pipeline.html
    //
    $.fn.dataTable.pipeline = function (opts) {
        // Configuration options
        var conf = $.extend({
            pages: 5,     // number of pages to cache. That means action(url) will be called in 1st, 6th, 11th ... pages
            url: '',  // url to controller action
            data: null,   // function or object with parameters to send to the server
            method: 'GET' // Ajax HTTP method
        }, opts);

        // Private variables for storing the cache
        var cacheLower = -1;
        var cacheUpper = null;
        var cacheLastRequest = null;
        var cacheLastJson = null;

        return function (request, drawCallback, settings) {
            var ajax = false;
            var requestStart = request.start;
            var drawStart = request.start;
            var requestLength = request.length;
            var requestEnd = requestStart + requestLength;

            if (settings.clearCache) {
                // API requested that the cache be cleared
                ajax = true;
                settings.clearCache = false;
            }
            else if (cacheLower < 0 || requestStart < cacheLower || requestEnd > cacheUpper) {
                // outside cached data - need to make a request
                ajax = true;
            }
            else if (JSON.stringify(request.order) !== JSON.stringify(cacheLastRequest.order) ||
                JSON.stringify(request.columns) !== JSON.stringify(cacheLastRequest.columns) ||
                JSON.stringify(request.search) !== JSON.stringify(cacheLastRequest.search)
            ) {
                // properties changed (ordering, columns, searching)
                ajax = true;
            }

            // Store the request for checking next time around
            cacheLastRequest = $.extend(true, {}, request);

            if (ajax) {
                // Need data from the server
                if (requestStart < cacheLower) {
                    requestStart = requestStart - (requestLength * (conf.pages - 1));

                    if (requestStart < 0) {
                        requestStart = 0;
                    }
                }

                cacheLower = requestStart;
                cacheUpper = requestStart + (requestLength * conf.pages);

                request.start = requestStart;
                request.length = requestLength * conf.pages;

                // Provide the same `data` options as DataTables.
                if (typeof conf.data === 'function') {
                    // As a function it is executed with the data object as an arg
                    // for manipulation. If an object is returned, it is used as the
                    // data object to submit
                    var d = conf.data(request);
                    if (d) {
                        $.extend(request, d);
                    }
                }
                else if ($.isPlainObject(conf.data)) {
                    // As an object, the data given extends the default
                    $.extend(request, conf.data);
                }

                settings.jqXHR = $.ajax({
                    "type": conf.method,
                    "url": conf.url,
                    "data": request,
                    "dataType": "json",
                    "cache": false,
                    "success": function (json) {
                        console.log(json);
                        cacheLastJson = $.extend(true, {}, json);

                        if (cacheLower != drawStart) {
                            json.data.splice(0, drawStart - cacheLower);
                        }
                        if (requestLength >= -1) {
                            json.data.splice(requestLength, json.data.length);
                        }

                        drawCallback(json);
                    }
                });
            }
            else {
                json = $.extend(true, {}, cacheLastJson);
                json.draw = request.draw; // Update the echo for each response
                json.data.splice(0, requestStart - cacheLower);
                json.data.splice(requestLength, json.data.length);

                drawCallback(json);
            }
        }
    };

    // Register an API method that will empty the pipelined data, forcing an Ajax
    // fetch on the next draw (i.e. `table.clearPipeline().draw()`)
    // Copied from https://datatables.net/examples/server_side/pipeline.html
    $.fn.dataTable.Api.register('clearPipeline()', function () {
        return this.iterator('table', function (settings) {
            settings.clearCache = true;
        });
    });


});
