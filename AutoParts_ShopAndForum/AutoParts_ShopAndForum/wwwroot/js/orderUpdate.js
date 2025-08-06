$(document).ready(function() {
    function toggleDateDelivery() {
        console.log("toggle fired")
        
        if ($("#isDeliveredCheckbox").is(":checked")) {
            $("#dateDeliveryContainer").show();
        } else {
            $("#dateDeliveryContainer").hide();
            $("#DateDelivery").val("");
        }
    }

    toggleDateDelivery();

    $("#isDeliveredCheckbox").change(function() {
        toggleDateDelivery();
    });
});