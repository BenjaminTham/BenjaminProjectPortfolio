
//alertify.defaults = {
//    // notifier defaults
//    notifier: {
//        // auto-dismiss wait time (in seconds)  
//        delay: 5,
//        // default position
//        position: 'bottom-right',
//        // adds a close button to notifier messages
//        closeButton: true,
//    },
//};

$(document).on('click', '[su-delete]', function () {
    var id = $(this).attr("su-data-id");
    var action = $(this).attr("su-data-action");
    itemDelete(id, action);
});

function itemDelete(id, action) {
    console.log(id, action);
    swal({
        title: "Delete Confirm?",
        text: "Are you wanted to delete the item?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
        reverseButtons: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax(action, {
                type: 'POST',
                data: { id: id },
                success: function (response, status, xhr) {
                    location.reload();
                },
                error: function (jqXhr, textStatus, errorMessage) {
                    console.log(errorMessage);
                }
            });
        } else {
            //swal("Your file is safe!");
        }
    });

}

$(document).on('click', '[su-dttable-delete]', function () {
    var id = $(this).attr("su-data-id");
    var action = $(this).attr("su-data-action");
    dtTableItemDelete(id, action);
});

function dtTableItemDelete(id, action) {
    //var ItemDelete = document.querySelector('[su-data-ispermit-delete]').href;
    //console.log(ItemDelete);
    swal({
        title: "Delete Confirm?",
        text: "Are you wanted to delete '" + name + "'?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
        reverseButtons: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax(action, {
                type: 'POST',
                data: { id: id },
                success: function (response, status, xhr) {
                    console.log(response);
                    suTable.clearPipeline().draw(false);

                    notifier.open({ type: "success", message: "Item Deleted Successfully!" })
                },
                error: function (jqXhr, textStatus, errorMessage) {
                    console.log(errorMessage);
                }
            });
        } else {
            //swal("Your file is safe!");
        }
    });
}