"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/GameHub")
    .build();

connection.start().then(function () {
    let code = document.getElementById("code").innerText;
    console.log(code);
    connection.invoke("ConnectToRoomAsSpectator", code).catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function ChangeState(room) {
    document.getElementById("isStarted").innerText = room.isStarted;
    document.getElementById("players").innerText = room.players;
    document.getElementById("spectators").innerText = room.spectators;
    document.getElementById("map").innerText = room.gameMap.map;
}

connection.on("ChangeState", function (room) {
    ChangeState(room);
});

connection.on("ChangeStateWithAction", function (room, gameAction) {
    ChangeState(room);
});