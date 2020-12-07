"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/GameHub")
    .build();

connection.start().then(function () {
    var code = document.getElementById("code").innerText;
    console.log(code);
    connection.invoke("ConnectToRoomAsSpectator", code).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("ChangeState", function (room) {
    var li = document.createElement("li");
    li.textContent = "ChangeState";
    document.getElementById("SpectatorsList").appendChild(li);
});

connection.on("ChangeStateWithAction", function (room, gameAction) {
    var li = document.createElement("li");
    li.textContent = "ChangeStateWithAction";
    document.getElementById("SpectatorsList").appendChild(li);
});