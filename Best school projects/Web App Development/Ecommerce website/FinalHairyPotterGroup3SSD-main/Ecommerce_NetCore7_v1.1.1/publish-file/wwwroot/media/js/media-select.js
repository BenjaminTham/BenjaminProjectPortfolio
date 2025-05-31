var pageSize = 20;
var pageIndex = 0;
var selectedMedia;
var selectedInput;
var isMultipleAllowed = false;
var mediaArray = [];
var selectedMediasId;


$(function () {
    $(document).on('click', '[su-media-popup]', function () {
        $('.image-checkbox-checked').each(function () {
            $(this).removeClass('image-checkbox-checked');
        });
        mediaArray = [];
        selectedInput = $(this);
        isMultipleAllowed = selectedInput.siblings('input[su-media-selected-input]').attr('su-media-selected-input') == 'multiple' ? true : false;
        
        $("#suMediaModal").modal('show');
        GetData();
    });

    $(document).on('click', '.load-more', function () {
        GetData();
    });
});

function GetData() {
    $.ajax({
        type: 'GET',
        url: '/getpagedmedia',
        data: { "pageindex": pageIndex, "pagesize": pageSize },
        dataType: 'json',
        success: function (data) {
            console.log(data);
            if (data.length != 0) {
                //console.log("Data Available");
                for (var i = 0; i < data.length; i++) {
                    $('<label class="image-checkbox m-2"><img class="img-fluid" su-media-id="' + data[i].id + '" src="/' + data[i].name + '" alt="User picture" style="max-height: 100px;max-width:-webkit-fill-available"><i class="fa fa-check"></i></label>').hide().appendTo(".media-image").fadeIn(1000);
                }
                pageIndex++;
            } else {
                //console.log("Data Not Available");
                $("#nomoredata").show();
            }
        },
        beforeSend: function () {
            $(".load-more").text("Loading...");
        },
        complete: function () {
            $(".load-more").text("Load More");
        },
        error: function () {
            alert("Error while retrieving data!");
        }
    });
}

function workaroundDropzone() {
    return "files";
}
Dropzone.autoDiscover = false;
var mediaDropzone = new Dropzone("#mediaDropzone", {
    paramName: workaroundDropzone,
    autoProcessQueue: false,
    acceptedFiles: "image/*,application/pdf",
    parallelUploads: 30, // Number of files process at a time (default 2)
    //maxFilesize: 1, // MB
    //maxFiles: 30,
    uploadMultiple: true,
    dictRemoveFile: 'Remove',
    //dictFileTooBig: 'Image is larger than 1MB',
    addRemoveLinks: true,
    error: function (file, errorMessage) {
        errors = true;
        console.log(errorMessage);
        var err = [];
        err.push(errorMessage);
        renderValidationError(err);
    },
    //success: function (file, response) {
    //    alert(JSON.stringify(response));
    //},

});

mediaDropzone.on("success", function (file) {
    setTimeout(function () { mediaDropzone.removeFile(file); }, 1000);
    $('[href="#nav-gallery"]').tab('show');
});

mediaDropzone.on("queuecomplete", function (file) {
    $(".media-image").empty();
    pageIndex = 0;
    GetData();
});

$('#btnSubmit').click(function () {
    mediaDropzone.processQueue();
});


jQuery(function ($) {
    $(document).on('click', '.image-checkbox', function (e) {
        var selected = $(this).find('img').attr('su-media-id');
        if ($(this).hasClass('image-checkbox-checked')) {
            $(this).removeClass('image-checkbox-checked');
            // remove deselected item from array
            mediaArray = $.grep(mediaArray, function (value) {
                return value != selected;
            });
        }
        else
        {
            if (isMultipleAllowed == false) {
                $('.image-checkbox-checked').each(function () {
                    $(this).removeClass('image-checkbox-checked');
                });
                mediaArray = [];
                mediaArray.push(selected);
            } else {
                if (mediaArray.indexOf(selected) === -1) {
                    mediaArray.push(selected);
                }
            }
            $(this).addClass('image-checkbox-checked');
        }
        getById(selected);
        console.log(mediaArray.length);
        if (mediaArray.length > 0) {
            $("#selectMedia").html('<button type="button" id="selectMediaBtn" class="btn btn-sky px-5" data-bs-dismiss="modal">Select</button>');
        } else {
            $("#selectMedia").html('<span class="text-yellow">Please Select a Item</span>');
        }

        selectedMediasId = mediaArray.join(",");
        e.preventDefault();
    });
});


function getById(id) {
    $.ajax('/getmediabyid/' + id, {
        type: 'GET',
        success: function (response, status, xhr) {
            $('input[su-media-id]').val(response.id);
            $('input[su-media-name]').val(response.name);
            $('[su-media-name]').html(response.name);
            $('input[su-media-title]').val(response.title);
            $('input[su-media-tags]').val(response.tags);
            $('input[su-media-tags]').amsifySuggestags();

            selectedMedia = response;
        },
        error: function (jqXhr, textStatus, errorMessage) {
            console.log(errorMessage);
        }
    });
}

//add selected image value to image section from gallery
$(document).on('click', '#selectMediaBtn', function () {
    console.log(selectedMedia);
    selectedInput.closest('div[su-media-section]').find('img[su-media-preview]').attr("src", "/" + selectedMedia.name);
    selectedInput.closest('div[su-media-section]').find('input[su-media-selected-input]').val(selectedMediasId);
    selectedInput.closest('div[su-media-section]').addClass('set-selected-media');

});


//remove image after remove-image icon click
$(document).on('click', '[su-media-deselect]', function () {
    console.log(selectedMedia);
    var thisItem = $(this).closest('div[su-media-section]');
    thisItem.find('img[su-media-preview]').attr("src", "/media/images/no-image.png");
    thisItem.find('input[su-media-selected-input]').val('');
    thisItem.find('div').removeClass('set-selected-media');
});

$("#mediaUpdateForm").submit(function (event) {
    event.preventDefault();
    $f = $(event.currentTarget); // fetch the form object

    var url = $f.attr("action");
    var data = new FormData(this);
    $.ajax({
        url: url,
        method: "POST",
        cache: false,
        data: data,
        processData: false,
        contentType: false,
        enctype: 'multipart/form-data',
        success: function (response, status, xhr) {
            console.log(response);
        },
        error: function (jqXhr, textStatus, errorMessage) {
            console.log(errorMessage);
        },
        beforeSend: function () {
            $("#mediasavebtn").show();
        },
        complete: function (jqXHR, status) {
            $("#mediasavebtn").hide();
            if (status == "error") { notifier.open({ type: "error", message: jqXHR.statusText }); }
        }
    });
});


