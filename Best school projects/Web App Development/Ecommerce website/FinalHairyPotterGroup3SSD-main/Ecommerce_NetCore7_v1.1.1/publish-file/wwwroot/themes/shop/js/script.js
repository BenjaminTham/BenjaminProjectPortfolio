function convertNullableInt(n) {
    return n != null ? parseInt(n): null;
}

function convertNullOrZeroInt(n) {
    return n != null ? parseInt(n) : 0;
}

function convertNullableFloat(n) {
    return n != null ? parseFloat(n) : null;
}

function convertNullOrZeroFloat(n) {
    return n != null ? parseFloat(n) : 0;
}

function convertNullableString(str) {
    return (str != null && Boolean(str.trim())) ? str : null;
}

$(function () {
    let owl = $("#main-slider");
    if (owl.length) {
        owl.owlCarousel({
            navigation: true,
            slideSpeed: 300,
            paginationSpeed: 500,
            items: 1,
            singleItem: true,
            loop: true,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 800,
        });
    }
});


$(function () {
    let slider = $("#slider");
    let thumb = $("#thumb");
    let slidesPerPage = 4; //globaly define number of elements per page
    let syncedSecondary = true;
    if (slider.length) {
        slider.owlCarousel({
            items: 1,
            slideSpeed: 2000,
            nav: false,
            autoplay: false,
            dots: false,
            loop: true,
            responsiveRefreshRate: 200
        }).on('changed.owl.carousel', syncPosition);
    }
    if (thumb.length) {
        thumb.on('initialized.owl.carousel', function () {
            thumb.find(".owl-item").eq(0).addClass("current");
        }).owlCarousel({
            items: slidesPerPage,
            dots: false,
            nav: true,
            item: 4,
            smartSpeed: 200,
            slideSpeed: 500,
            slideBy: slidesPerPage,
            navText: ['<svg width="18px" height="18px" viewBox="0 0 11 20"><path style="fill:none;stroke-width: 1px;stroke: #000;" d="M9.554,1.001l-8.607,8.607l8.607,8.606"/></svg>', '<svg width="25px" height="25px" viewBox="0 0 11 20" version="1.1"><path style="fill:none;stroke-width: 1px;stroke: #000;" d="M1.054,18.214l8.606,-8.606l-8.606,-8.607"/></svg>'],
            responsiveRefreshRate: 100
        }).on('changed.owl.carousel', syncPosition2);
    }


    function syncPosition(el) {
        var count = el.item.count - 1;
        var current = Math.round(el.item.index - (el.item.count / 2) - .5);
        if (current < 0) {
            current = count;
        }
        if (current > count) {
            current = 0;
        }
        thumb
            .find(".owl-item")
            .removeClass("current")
            .eq(current)
            .addClass("current");
        var onscreen = thumb.find('.owl-item.active').length - 1;
        var start = thumb.find('.owl-item.active').first().index();
        var end = thumb.find('.owl-item.active').last().index();
        if (current > end) {
            thumb.data('owl.carousel').to(current, 100, true);
        }
        if (current < start) {
            thumb.data('owl.carousel').to(current - onscreen, 100, true);
        }
    }

    function syncPosition2(el) {
        if (syncedSecondary) {
            var number = el.item.index;
            slider.data('owl.carousel').to(number, 100, true);
        }
    }

    thumb.on("click", ".owl-item", function (e) {
        e.preventDefault();
        var number = $(this).index();
        slider.data('owl.carousel').to(number, 300, true);
    });


    $(".qtyminus").on("click",
        function () {
            var now = $(".qty").val();
            if ($.isNumeric(now)) {
                if (parseInt(now) - 1 > 0) {
                    now--;
                }
                $(".qty").val(now);
            }
        });
    $(".qtyplus").on("click", function () {
        var now = $(".qty").val();
        if ($.isNumeric(now)) {
            $(".qty").val(parseInt(now) + 1);
        }
    });
});


(function ($) {
    var methods;
    methods = {
        init: function (element, options) {
            element.addClass('stack-menu');
            element.find('ul').addClass('stack-menu__list');
            element.find('ul:first').addClass('stack-menu__list--root').addClass('stack-menu__list--active');
            element.find('li').addClass('stack-menu__item');
            element.find('a').addClass('stack-menu__link');
            $('.stack-menu__item').each(function (index) {
                $(this).attr('data-id', index);
                if ($(this).find('.stack-menu__list').length > 0) {
                    $(this).children('.stack-menu__link').addClass('stack-menu__link--parent');
                }
            });
            $('.stack-menu__list').each(function () {
                var allItemElement, allLinkElement, backItemElement, backLinkElement, url;
                if (!$(this).hasClass('stack-menu__list--root')) {
                    if (options.all) {
                        url = $(this).closest('.stack-menu__item').find('.stack-menu__link').attr('href');
                        allItemElement = $('<li>', {
                            "class": 'stack-menu__item'
                        });
                        allLinkElement = $('<a>', {
                            "class": 'stack-menu__link',
                            href: url,
                            text: options.allTitle
                        });
                        allItemElement.append(allLinkElement);
                        $(this).prepend(allItemElement);
                    }
                    backItemElement = $('<li>', {
                        "class": 'stack-menu__item'
                    });
                    backLinkElement = $('<a>', {
                        "class": 'stack-menu__link stack-menu__link--back',
                        href: 'javascript:void(0)',
                        //html: '&nbsp;'
                        //html: '<span style="display: flex;justify-content: space-between;"><span>&nbsp;</span><span>Back</span></span>'
                        html: '<span><span>&nbsp;</span>&nbsp;&nbsp;<span>Back</span></span>'
                    });
                    backItemElement.append(backLinkElement);
                    $(this).prepend(backItemElement);
                }
            });
            element.find('.stack-menu__link').click(function (event) {
                var item, link, list, parent, sub;
                link = $(this);
                item = link.closest('.stack-menu__item');
                list = item.closest('.stack-menu__list');
                parent = list.closest('.stack-menu__item');
                sub = item.children('.stack-menu__list');
                var pWidth = $(this).innerWidth(); //use .outerWidth() if you want borders
                var pOffset = $(this).offset();
                var x = event.pageX - pOffset.left;
                if (link.hasClass('stack-menu__link--back')) {
                    event.preventDefault();
                    list.removeClass('stack-menu__list--active');
                    list.removeClass('stack-menu__list--active');
                    parent.removeClass('stack-menu__item--opened');
                    parent.find('.stack-menu__link').removeClass('stack-menu__link--hidden');
                    parent.closest('.stack-menu__list').children('.stack-menu__item').removeClass('stack-menu__item--hidden');
                } else {
                    if (pWidth / 2 > x) {
                        window.location = $(this).attr('href');
                    } else {
                        if (item.children('.stack-menu__list').length === 0) {
                            return true;
                        } else {
                            event.preventDefault();
                            parent.addClass('stack-menu__item--opened');
                            link.addClass('stack-menu__link--hidden');
                            sub.addClass('stack-menu__list--active');
                            $(list.children('.stack-menu__item')).each(function () {
                                if ($(this).data('id') !== item.data('id')) {
                                    $(this).addClass('stack-menu__item--hidden');
                                }
                            });
                        }
                    }
                }
            });
        }
    };
    jQuery.fn.stackMenu = function (options) {
        options = $.extend({
            all: false,
            allTitle: 'All'
        }, options);
        methods.init(this, options);
        return {
            reset: function (element) {
                $(element).find('.stack-menu').removeClass('stack-menu--active');
                $(element).find('.stack-menu__list').removeClass('stack-menu__list--active');
                $(element).find('.stack-menu__item').removeClass('stack-menu__item--hidden').removeClass('stack-menu__item--opened');
                $(element).find('.stack-menu__link').removeClass('stack-menu__link--hidden');
                $(element).find('.stack-menu__list--root').addClass('stack-menu__list--active');
            }
        };
    };
})(jQuery);


$(function () {
    $("#stack-menu").stackMenu({
        all: false,
    });
});

$('.moreless-button').click(function () {
    $('.moretext').slideToggle();
    if ($('.moreless-button').html() == '<span>Show More</span><i class="fa fa-plus"></i>') {
        $(this).html('<span>Show Less</span><i class="fa fa-minus"></i>');
    } else {
        $(this).html('<span>Show More</span><i class="fa fa-plus"></i>');
    }
});

$("#stack-menu > ul > li").each(function () {
    var count = $(this).index();
    if (count > 6) {
        $(this).addClass('moretext');
        $('.moreless-button').removeClass('moreless-button-hidden');
    } else {
        $(this).removeClass('moretext');
        $('.moreless-button').addClass('moreless-button-hidden');
    }
    $('.moreless-button-li').removeClass('moretext');
});


// Mobile menu
"use strict";
let links = document.querySelectorAll(".mobile-menu a");
// Loop through the link lists
links.forEach((link) => {
    // Add a click event on each link
    link.addEventListener("click", () => {
        // Get current active link and store in currActive variable
        let currActive = document.querySelector(".active");
        // Set next active link to the current active classname
        let nextActive = currActive.className;
        // Set the current active class to none
        currActive.className = nextActive.replace("active", "");
        // Set the clicked link item to active.
        link.className += "active";
    });
});

/*Off Canvas - Start*/
jQuery(document).ready(function ($) {
    $(document).on('click', '.pull-bs-canvas-right, .pull-bs-canvas-left', function () {
        $('body').prepend('<div class="bs-canvas-overlay bg-dark position-fixed w-100 h-100"></div>');
        if ($(this).hasClass('pull-bs-canvas-right'))
            $('.bs-canvas-right').addClass('mr-0');
        else
            $('.bs-canvas-left').addClass('ml-0');
        return false;
    });

    $(document).on('click', '.bs-canvas-close, .bs-canvas-overlay', function () {
        var elm = $(this).hasClass('bs-canvas-close') ? $(this).closest('.bs-canvas') : $('.bs-canvas');
        elm.removeClass('mr-0 ml-0');
        $('.bs-canvas-overlay').remove();
        return false;
    });
});
/*Off Canvas - End*/

/*Response-Menu - Start*/
const mainNavigation = document.querySelector(".main-navigation");
if (mainNavigation) {
    const overlay = mainNavigation.querySelector(".overlay");
    const toggler = mainNavigation.querySelector(".navbar-toggler");
    const openSideNav = () => mainNavigation.classList.add("active");
    const closeSideNav = () => mainNavigation.classList.remove("active");
    toggler.addEventListener("click", openSideNav);
    overlay.addEventListener("click", closeSideNav);
}
/*Response-Menu - End*/

$(function() {
    $('.mobile-menu a').each(function() {
        let hrefUrl = $(this).attr("href");
        let pathUrl = window.location.pathname;
        if (hrefUrl.toLowerCase() == pathUrl.toLowerCase()) {
            $(this).addClass('active');
        }
    });
});

//cart count ViewComponent
function CartCount() {
    $.ajax({
        url: "/getcartcount",
        type: "get",
        dataType: "html",
        //beforeSend: function (x) {
        //},
        //data: null,
        success: function (result) {
            $(".total-cart-count").html(result);
        }
    });
}

//cart ViewComponent
function GetCart() {
    $.ajax({
        url: "/getcart",
        type: "get",
        dataType: "html",
        success: function (result) {
            $(".cart-component").html(result);
        }
    });
}

//cart summary ViewComponent
function CartSummary() {
    $.ajax({
        url: "/getcartsummary",
        type: "get",
        dataType: "html",
        success: function (result) {
            $(".cart-summary").html(result);
        }
    });
}

//checkoutorderpreview ViewComponent
function GetCheckoutOrderPreview() {
    let checkoutSummarySection = $(".checkout-order-summary");
    if (checkoutSummarySection.length) {
        $.ajax({
            url: "/getcheckoutorderpreview",
            type: "get",
            dataType: "html",
            success: function (result) {
                $(".checkout-order-summary").html(result);
                calDeliveryCharge();
                calTotalCharge();
            }
        });
    }
}

function ItemIncrement(variantId) {
    $.ajax({
        url: "/cartitemincrement/" + variantId,
        type: "get",
        dataType: "html",
        success: function (result) {
            CartSummary();
            CartCount();
            GetCart();
            updateCartValueByVariant(variantId);
            GetCheckoutOrderPreview();
        }
    });
}


function ItemDecrement(variantId) {
    $.ajax({
        url: "/cartitemdecrement/" + variantId,
        type: "get",
        dataType: "html",
        success: function (result) {
            CartSummary();
            CartCount();
            GetCart();
            updateCartValueByVariant(variantId);
            GetCheckoutOrderPreview();
        }
    });
}

function CartItemRemove(variantId) {
    $.ajax({
        url: "/cartitemremove/" + variantId,
        type: "get",
        dataType: "html",
        success: function (result) {
            CartSummary();
            CartCount();
            GetCart();
            updateCartValueByVariant(variantId);
            GetCheckoutOrderPreview();
        }
    });
}

//function getCartCount() {
//    var cartCount = 0;
//    const shopCart = getCookie('shop-cart');
//    if (shopCart) {
//        var cartList = JSON.parse(shopCart);
//        cartCount = cartList.reduce((accumulator, item) => accumulator + item.Qty, 0);
//    }
//    return cartCount;
//}

function getCartList() {
    var cart = [];
    const shopCart = getCookie('shop-cart');
    if (shopCart) {
        cart = JSON.parse(shopCart);
    }
    return cart;
}

function getCartByVariant(variantId) {
    var variant = null;
    const shopCart = getCookie('shop-cart');
    if (shopCart) {
        const cart = JSON.parse(shopCart);
        variant = cart.find(x => x.VariantId == variantId);
    }
    return variant;
}

function updateCartValueByVariant(variantId) {
    let addToCartBtn = $(".add-to-cart-button button");

    if (addToCartBtn.length) {
        const btnVariantId = $('.add-to-cart-button button').attr("product-variant-id");
        const shopCart = getCookie('shop-cart');
        const cart = JSON.parse(shopCart);
        if (shopCart) {
            var variant = cart.find(x => x.VariantId == variantId);

            if (btnVariantId == variantId && variant == null) {
                $('.add-to-cart-qty').val(0);
                $('.add-to-cart-adjust').attr("style", "display:none");
                $('.add-to-cart-button button').attr("style", "display:inherit");
            }

            if (variant != null) {
                if (btnVariantId == variant.VariantId) {
                    if (variant.Qty > 0) {
                        $('.add-to-cart-qty').val(variant.Qty);
                    }
                } 
            } 
        }
    }
}


function addToWishList(id, wished = false, cookieName) {
    let wishCookie = getCookie(cookieName);

    let wishCookieIds = [];
    wishCookieIds = wishCookie == null ? wishCookieIds : wishCookie?.toString().split(",").map(Number);

    //let wishCookieIds = JSON.parse(myArray)??[];

    if (wished) {
        wishCookieIds.push(id);
    } else {
        let indexToRemove = wishCookieIds.indexOf(id);
        if (indexToRemove !== -1) {
            wishCookieIds.splice(indexToRemove, 1);
        }
    }

    setCookie(cookieName, wishCookieIds.toString(), 30);
    //updateWish();
}



function getCookie(name) {
    const cookieName = name + "=";
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookiesArray = decodedCookie.split(';');
    for (let i = 0; i < cookiesArray.length; i++) {
        let cookie = cookiesArray[i];
        while (cookie.charAt(0) === ' ') {
            cookie = cookie.substring(1);
        }
        if (cookie.indexOf(cookieName) === 0) {
            return cookie.substring(cookieName.length, cookie.length);
        }
    }
    return "";
}

function setCookie(name, value, days) {
    const date = new Date();
    date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
    const expires = "expires=" + date.toUTCString();
    document.cookie = name + "=" + value + ";" + expires + ";path=/";
}

function deleteCookie(name) {
    document.cookie = name + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}

// Set a cookie with the name "cookieName", value "cookieValue", and expiration of 30 days
//setCookie("cookieName", "cookieValue", 30);

// Get the value of the cookie with the name "cookieName"
//const cookieValue = getCookie("cookieName");

// Delete the cookie with the name "cookieName"
//deleteCookie("cookieName");


$("#globalSearch").keyup(function () {
    var search = $(this).val() == '' ? null : $(this).val();
    if (search != null && search.length > 2) {
        var result = globalSearch(search);
        $('#globalSearchResult').html(result);
    } else {
        $('#globalSearchResult').html('');
    }
});

function globalSearch(searchValue) {
    var response;
    $.ajax({
        url: "/Product/ProductSearch/" + searchValue,
        type: 'GET',
        async: false,
        dataType: 'json',
        success: function (result) {
            console.log(result);
            var li = '';

            $.each(result, function () {
                li += `<a href="/productdetails/${this.productSlug}" class="d-inline-block text-decoration-none text-dark" style="border-bottom: 1px solid #f5f5f5;padding: 4px 0px;">
                            <img class="me-1" src="${(this.imagePreview != null ? "/" + this.imagePreview : "/assets/images/no-image.png") }" alt="image_not_found" style="height: 40px; width: 40px; object-fit: cover;">
                            <div class="d-inline-block lh-sm">
                                <small class="d-block" class="btn btn-link p-0 text-start fw-bold align-top">${this.productName}</small>
                                <small class="d-block"><strong>Category: </strong><span style="color:#8BC34A;">${this.productCategory}</span></small>
                            </div>
                        </a>`;
            });
            response = li;
        },
        error: function (e) {
            console.log(e);
        }
    });
    return response;
}