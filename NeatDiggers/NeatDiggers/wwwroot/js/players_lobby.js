"use strict";

import * as game from "./game.js";
import { Conection, getKeyByValue, Message, WeaponType } from "./util.js";

let connection;

window.onload = async function(){
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/GameHub")
        .build();

    $("#StartGame").prop("disabled", true);
    $("#ChangeReady").prop("disabled", true);

    connection.start().then(async function () {
        let user = {
            name: $("#name").text(),
            code: $("#code").text(),
            token: $("#token").text(),
        }
        let room = await connection.invoke("ConnectToRoom", user).catch(function (err) {
            return console.error(err.toString());
        });

        if (room == null) {
            $("#errorModal").modal();
            $("#errorModalMessage").text(Conection.Error.WrongCode.Description);
            return;
        }

        let userId = await connection.invoke("GetUserId").catch(function (err) {
            return console.error(err.toString());
        });

        console.log(`Player ${name} connected to lobby: ${code}`);

        await game.init(connection, userId);
        UpdateRoom(room, null);
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

function UpdateRoom(room, action) {
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

async function SelectCharacter(name) {
    let character = await connection.invoke("ChangeCharacter", parseInt(name)).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById("ChangeReady").disabled = false;

    let div = $("#info")[0];

    while (div.firstChild)
        div.removeChild(div.firstChild);

    let maxHealth = document.createElement("p");
    maxHealth.innerText = Message.Health + character.maxHealth;
    let weaponType = document.createElement("p");
    weaponType.innerText = Message.WeaponType + getKeyByValue(WeaponType, character.weaponType);
    let abilities = document.createElement("div");

    for (var i = 0; i < character.abilities.length; i++) {
        let description = document.createElement("p");
        description.innerText = "Level [" + i + "] " + character.abilities[i].description;
        abilities.appendChild(description);
    }

    div.append(maxHealth);
    div.append(weaponType);
    div.append(abilities);
}

function ChangeReady() {
    connection.invoke("ChangeReady").catch(function (err) {
        return console.error(err.toString());
    });
    $('.SelectCharacter').prop('disabled', function (i, v) { return !v; });
}

async function StartGame() {
    connection.invoke("StartGame").catch(function (err) {
        return console.error(err.toString());
    });
}