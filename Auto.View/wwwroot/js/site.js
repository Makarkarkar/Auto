$(document).ready(connectToSignalR);

function displayNotification(user, json) {
    var $target = $('div#signalr-notifications');

    var data = JSON.parse(json);

    var message =
        `NEW ONWER: ${data.name} ${data.surname}, 
            ${data.email},
            ${data.age}, ${data.modelCode},
             ${data.registration}, ${data.year}`;

    var $div = $(`<div>${user}: ${message}</div>`);
    $target.prepend($div);
    window.setTimeout(function () { $div.fadeOut(2000, function () { $div.remove(); }); }, 8000);
}

/*
function displayNotification(user, json) {
    var $target = $('div#signalr-notifications');
    var data = JSON.parse(json);
    var message =
        `NEW VEHICLE! <a href="/vehicles/details/${data.registration}">${data.registration}</a> (${data.manufacturerName} ${data.modelName},
            ${data.color}, ${data.year}).
            Price ${data.currencyCode} ${data.price}`;
    var $div = $(`<div>${message}</div>`);
    $target.prepend($div);
    window.setTimeout(function () { $div.fadeOut(2000, function () { $div.remove(); }); }, 8000);
}
*/

function connectToSignalR() {
    console.log("Connecting to SignalR...");
    window.notificationDivs = new Array();
    var conn = new signalR.HubConnectionBuilder().withUrl("/hub").build();
    conn.on("DisplayNotification", displayNotification);
    conn.start().then(function () {
        console.log("SignalR has started.");
    }).catch(function (err) {
        console.log(err)
    });
}