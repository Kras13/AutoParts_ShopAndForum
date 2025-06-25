function onDeliverySetToPersonalAddress() {
    console.log("To home address triggered")

    $("#homeAddressComp").show();
    $("#courierAddressComp").hide();
}

function onDeliverySetToCourierAddress() {
    console.log("to courier address triggered")

    $("#courierAddressComp").show();
    $("#homeAddressComp").hide();
}

$(document).ready(function () {
    console.log('OKOKOK');
    var townsDropDown = $("#townsDropDown");
    var stationsDropDown = $("#courierStationsDropDown");

    townsDropDown.ready(function () {
        LoadStations();
    });

    townsDropDown.on("change", function () {
        console.log("town changed");
        LoadStations();
    });

    var LoadStations = function () {
        var townId = townsDropDown.val();

        if (townId == -1)
            return;

        console.log('loading stations for town with id: ' + townId);

        $.ajax({
            type: "GET",
            url: "/Checkout/GetCourierStationsForTown/" + townId,
            dataType: "json",
            success: function (data) {
                stationsDropDown.empty();
                $.each(data, function () {
                    stationsDropDown.append("<option value='" + this.id + "'>" + this.displayName + "</option>");
                    console.log(data[0].displayName);
                });
            }
        });
    }
});