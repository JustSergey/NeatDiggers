"use strict";

import * as game from "./game.js";
import { Conection } from "./util.js";

let connection;

window.onload = async function () {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/GameHub")
        .build();

    connection.start().then(async function () {
        let code = $("#code").text();
        $("#codeInput").val(code);
        let room = await connection.invoke("ConnectToRoomAsSpectator", code).catch(function (err) {
            return console.error(err.toString());
        });

        if (room == null) {
            $("#errorModal").modal();
            $("#errorModalMessage").text(Conection.Error.WrongCode.Description);
            return;
        }

        console.log(`Spectator connected to lobby: ${code}`);

        await game.init(connection, "spec");
        UpdateRoom(room, null);
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ChangeState", function (room) {
        UpdateRoom(room, null);
        console.log("ChangeState");
    });

    connection.on("ChangeStateWithAction", function (room, gameAction) {
        UpdateRoom(room, gameAction);
        console.log("ChangeStateWithAction");
    });
};

function UpdateRoom(room, action)  {
    if (room.isStarted) {
        $("#game").show();
        $("#lobby").hide();
        $("#footer").hide();

        game.updateRoom(room, action);
    }
    else {
        $("#game").hide();
        LoadPlayers(room.players);
        $("#isStarted").text(room.isStarted);
        $("#spectators").text(room.spectators.length);
    }
}

function LoadPlayers(roomPlayers) {
    var players = document.getElementById("players");

    while (players.firstChild) {
        players.removeChild(players.firstChild);
    }

    for (var i = 0; i < roomPlayers.length; i++) {
        var playerContainer = document.createElement('div')
        playerContainer.textContent = roomPlayers[i].name + ' ' + roomPlayers[i].character.title + ' [' + roomPlayers[i].isReady + ']';
        players.appendChild(playerContainer)
    }
}