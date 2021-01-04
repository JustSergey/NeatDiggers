"use strict";

import * as game from "../js/game.js";
let connection;
window.onload = async function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/GameHub")
        .build();

    ToggleRendering('game', false);

    connection.start().then(function () {
        let code = document.getElementById("code").innerText;
        console.log(code);
        connection.invoke("ConnectToRoomAsSpectator", code).catch(function (err) {
            return console.error(err.toString());
        });
        console.log(`Spectator connected to lobby: ${code}`);
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ChangeState", function (room) {
        UpdateRoom(room);
        console.log("ChangeState");
    });

    connection.on("ChangeStateWithAction", function (room, gameAction) {
        UpdateRoom(room);
        console.log("ChangeStateWithAction");
    });
};

function UpdateRoom(room) {
    if (room.isStarted) {
        ToggleRendering('game', true);
        ToggleRendering('lobby', false);
        ToggleRendering('footer', false);

        game.animate();
        game.drawMap(room.gameMap);
    }
    else {
        LoadPlayers(room.players);
        document.getElementById("isStarted").innerText = room.isStarted;
        document.getElementById("spectators").innerText = room.spectators.length;
        document.getElementById("StartGame").disabled = !PlayersIsReady(room.players);
    }
}

function ToggleRendering(id, bool) {
    var x = document.getElementById(id);
    if (bool) {
        x.style.display = 'block';
    } else {
        x.style.display = 'none';
    }
}