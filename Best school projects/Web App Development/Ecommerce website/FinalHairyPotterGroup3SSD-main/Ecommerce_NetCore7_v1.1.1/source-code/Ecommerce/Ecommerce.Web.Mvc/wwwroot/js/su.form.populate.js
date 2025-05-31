(function ($) {
    $.fn.setFormData = function (data) {
        let t = this;

        $.each(data, function (key, value) {
            //debugger
            if (typeof data[key] != "object") {
                key = toPascalCase(key);
            }
            
            var ctrl = $(t).find('[name=' + key + ']');
            

            console.log(key);
            //console.log(value);
            //console.log(ctrl);
            //console.log(typeof data[value] == "object");
            if (typeof data[key] == "object") {
                $.each(value, function (key2, value2) {
                    key2 = toPascalCase(key2);
                    var ctrl2 = $(t).find('[name="' + key + '.' + key2 + '"]'); //This will work for something nested, like Product.Name

                    /*console.log(key + '.' + key2, value2)*/
                    //console.log(ctrl2)
                    switch (ctrl2.prop("type")) {
                        case "radio": case "checkbox":
                            ctrl2.each(function () {
                                ctrl2.attr("checked", value2);
                            });
                            break;
                        case "select":
                            // manipulate select
                            ctrl2.val(value2);
                            break;
                        default:
                            if (ctrl.hasClass("flatpickr-input") && value2 != null) {
                                var getdate = dayjs(value).format('YYYY-MM-DD');
                                ctrl.val(getdate);
                                flatpickr($("input[name=" + key2 + "]"), {
                                    defaultDate: value2,
                                });
                                break;
                            };
                            ctrl2.val(value2);
                            initSelect();
                    }
                });
            } else {
                //console.log(ctrl, key, value);
                switch (ctrl.prop("type")) {
                    case "radio": case "checkbox":
                        ctrl.each(function () {
                            ctrl.attr("checked", value);
                        });
                        break;
                    case "select":
                        // manipulate select
                        ctrl.val(value);
                        break;
                    case "file":
                        break;
                    default:
                        if (ctrl.hasClass("flatpickr-input") && value != null){
                            var getdate = dayjs(value).format('YYYY-MM-DD');
                            ctrl.val(getdate);
                            flatpickr($("input[name=" + key + "]"), {
                                defaultDate: value,
                            });
                            break;
                        };

                        ctrl.val(value);
                        hierarchySelect(value);
                }
            }

            function hierarchySelect(value) {
                var element = ctrl.closest('.hierarchy-select');
                var menuitems = element.find('.hs-menu-inner');

                $('.hs-menu-inner a').each(function () {
                    $(this).removeClass('active');
                    $(this).removeAttr('data-default-selected');
                    if ($(this).is(':first-child')) {
                        $(this).addClass('active');
                        console.log($(this).text());
                        $('.hierarchy-select>button').html($(this).text());
                    }
                });

                var hiddenFieldValue = element.children('input').val();
                if (value == hiddenFieldValue) {
                    var selected = element.find('a[data-value=' + hiddenFieldValue + ']');

                    if (hiddenFieldValue && hiddenFieldValue != 'undefined' && hiddenFieldValue.length > 0) {
                        menuitems.find('a').removeClass('active').removeAttr('data-default-selected');
                        element.find('button').html(selected.text());
                        selected.addClass('active');
                    } else {
                        
                    }
                }
            };
        });


    }
})(jQuery);