"use strict";

import * as game from "./game.js";
import { Message } from "./util.js";

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

        switch (userId) {
            case Message.ConnectionError.Full.Code:
                $("#errorModal").modal();
                $("#errorModalMessage").text(Message.ConnectionError.Full.Description);
                return;
            case Message.ConnectionError.Started.Code:
                $("#errorModal").modal();
                $("#errorModalMessage").text(Message.ConnectionError.Started.Description);
                return;
            case Message.ConnectionError.WrongCode.Code:
                $("#errorModal").modal();
                $("#errorModalMessage").text(Message.ConnectionError.WrongCode.Description);
                return;
        }

        console.log(`Player ${name} connected to lobby: ${code}`);

        game.init(connection, userId);
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("ChangeState", async function (room) {
        UpdateRoom(room, null);
        console.log("ChangeState");
    });

    connection.on("ChangeStateWithAction", async function (room, gameAction) {
        UpdateRoom(room, gameAction);
        console.log("ChangeStateWithAction");
    });

    window.SelectCharacter = SelectCharacter;
    window.ChangeReady = ChangeReady;
    window.StartGame = StartGame;
};

async function UpdateRoom(room, action) {
    if (room.isStarted) {
        $("#game").show();
        $("#lobby").hide();
        $("#footer").hide();

        game.updateRoom(room, action);
    }
    else {
        LoadPlayers(room.players);
        $("#isStarted").text(room.isStarted);
        $("#spectators").text(room.spectators.length);
        $("#StartGame").prop("disabled", !PlayersIsReady(room.players));
    }
    console.log(JSON.stringify(room));
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
    let ready = roomPlayers.length > 1;
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