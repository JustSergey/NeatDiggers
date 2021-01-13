"use strict";

import * as game from "./game.js";

let connection;

window.onload = async function(){
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/GameHub")
        .build();

    $("#StartGame").prop("disabled", true);
    $("#ChangeReady").prop("disabled", true);

    connection.start().then(async function () {
        let code = $("#code").text();
        let name = $("#name").text();
        let userId = await connection.invoke("ConnectToRoom", code, name).catch(function (err) {
            return console.error(err.toString());
        });
        console.log(`Player ${userId} connected to lobby: ${code}`);

        game.init(connection, userId);
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ChangeState", async function (room) {
        UpdateRoom(room);
        console.log("ChangeState");
    });

    connection.on("ChangeStateWithAction", async function (room, gameAction) {
        UpdateRoom(room);
        console.log("ChangeStateWithAction");
    });

    window.SelectCharacter = SelectCharacter;
    window.ChangeReady = ChangeReady;
    window.StartGame = StartGame;
};

async function UpdateRoom(room) {
    if (room.isStarted) {
        $("#game").show();
        $("#lobby").hide();
        $("#footer").hide();

        //await game.guiInit();

        game.updateRoom(room);
        //game.UpdateRoom(room, connection);
    }
    else {
        LoadPlayers(room.players);
        $("#isStarted").text(room.isStarted);
        $("#spectators").text(room.spectators.length);
        $("#StartGame").prop("disabled", !PlayersIsReady(room.players));
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

function PlayersIsReady(roomPlayers) {
    let ready = roomPlayers.length > 0;
    for (var i = 0; i < roomPlayers.length; i++) {
        ready = ready && roomPlayers[i].isReady;
    }
    return ready;
}

function SelectCharacter(name) {
    connection.invoke("ChangeCharacter", parseInt(name)).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("ChangeReady").disabled = false;
}

function ChangeReady() {
    connection.invoke("ChangeReady").catch(function (err) {
        return console.error(err.toString());
    });
}

async function StartGame() {
    connection.invoke("StartGame").catch(function (err) {
        return console.error(err.toString());
    });
}