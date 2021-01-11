"use strict";

import * as game from "../js/game.js";

let userId;
let connection;

window.onload = async function(){
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/GameHub")
        .build();

    document.getElementById("StartGame").disabled = true;
    document.getElementById("ChangeReady").disabled = true;
    ToggleRendering('game', false);

    connection.start().then(async function () {
        let code = document.getElementById("code").innerText;
        userId = await connection.invoke("ConnectToRoom", code, "WebPlayer").catch(function (err) {
            return console.error(err.toString());
        });
        console.log(`Player ${userId} connected to lobby: ${code}`);

        var gameMap = await connection.invoke("GetGameMap").catch(function (err) {
            return console.error(err.toString());
        });

        game.DrawMap(gameMap);
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
        await game.guiInit();
        ToggleRendering('game', true);
        ToggleRendering('lobby', false);
        ToggleRendering('footer', false);

        game.animate();
        room.userId = userId;
        game.UpdateRoom(room, connection);
        game.setTurn(room.players[room.playerTurn].id == userId);
    }
    else {
        LoadPlayers(room.players);
        document.getElementById("isStarted").innerText = room.isStarted;
        document.getElementById("spectators").innerText = room.spectators.length;
        document.getElementById("StartGame").disabled = !PlayersIsReady(room.players);
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

    console.log("players is ready:" + ready);
    return ready;
}

function SelectCharacter(name) {
    document.getElementById("ChangeReady").disabled = false;
    connection.invoke("ChangeCharacter", 1).catch(function (err) {
        return console.error(err.toString());
    });
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

function ToggleRendering(id, bool) {
    var x = document.getElementById(id);
    if (bool) {
        x.style.display = 'block';
    } else {
        x.style.display = 'none';
    }
}