// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const connection = new signalR.HubConnectionBuilder()
    .withUrl(appSettings.NotificationServer.Endpoint + "/SimulatedLongRunningTaskHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    console.log("Connected to SimulatedLongRunningTaskHub");
    toastr.info("Connected to SimulatedLongRunningTaskHub")
});

connection.on("ReceiveTaskStatus", (message) => {
    console.log("Received Message from Notification Server: </br>" + message);
    toastr.info("Received Message from Notification Server: </br>" + message)
});