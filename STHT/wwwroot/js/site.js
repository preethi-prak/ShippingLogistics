// Write your JavaScript code.
$(document).ready(function() {
    // Open Bid modal on button click
    $('#bidModalBtn').click(function () {
        $('#bidModal').modal('show');
    });

    // Open Shipping modal on link click
    $('#shippingModalLink').click(function () {
        $('#shippingModal').modal('show');
    });

    $('.close').click(function () {
        $('#bidModal').modal('hide');
        $('#shippingModal').modal('hide');
    });

    $(document).keydown(function (event) {
        if (event.keyCode == 27) {
            $('#bidModal').modal('hide');
            $('#shippingModal').modal('hide');
        }
    });
});


