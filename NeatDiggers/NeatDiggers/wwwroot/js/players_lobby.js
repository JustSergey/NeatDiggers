"use strict";

let connection = new signalR.HubConnectionBuilder()
    .withUrl("/GameHub")
    .build();

document.getElementById("StartGame").disabled = true;
document.getElementById("ChangeReady").disabled = true;
ToggleRendering('game', false);


connection.start().then(function () {
    let code = document.getElementById("code").innerText;
    console.log("Start:" + code);
    connection.invoke("ConnectToRoom", code, "WebPlayer").catch(function (err) {
        return console.error(err.toString());
    });
}).catch(function (err) {
    return console.error(err.toString());
});

function ChangeState(room) {
    document.getElementById("isStarted").innerText = room.isStarted;
    ToggleRendering('game', room.isStarted);
    ToggleRendering('lobby', !room.isStarted);
    document.getElementById("spectators").innerText = room.spectators.length;
    LoadPlayers(room.players);

    if (room.isStarted) {
        DrawMap(document.getElementById("map"), room.gameMap);
    }
    else {
        document.getElementById("StartGame").disabled = !PlayersIsReady(room.players);
    }
    room.gameMap = "";
    console.log(JSON.stringify(room, null, '\t'));
}

connection.on("ChangeState", function (room) {
    console.log("ChangeState");
    ChangeState(room);
});

connection.on("ChangeStateWithAction", function (room, gameAction) {
    console.log("ChangeStateWithAction");
    ChangeState(room);
});

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

    console.log("players is ready:" + ready);
    return ready;
}

function SelectCharacter(name) {
    document.getElementById("ChangeReady").disabled = false;
    console.log("name: " + name);
    //!!!!!!!!!!! передаю не enum !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    connection.invoke("ChangeCharacter", 1).catch(function (err) {
        return console.error(err.toString());
    });
}

function ChangeReady() {
    connection.invoke("ChangeReady").catch(function (err) {
        return console.error(err.toString());
    });
    console.log("ChangeReady()");
}

function StartGame() {
    connection.invoke("StartGame").catch(function (err) {
        return console.error(err.toString());
    });
    console.log("StartGame()");
}


function ToggleRendering(id, bool) {
    var x = document.getElementById(id);
    if (bool) {
        x.style.display = 'block';
    } else {
        x.style.display = 'none';
    }
}

function DrawMap(canvas, map) {
    let ctx = canvas.getContext("2d");
    var widthStep = canvas.width / map.width;
    var heightStep = canvas.height / map.height;
    ctx.font = '48px serif';
    ctx.fillRect(0, 0, map.height, map.width);
    for (var i = 0; i < map.width; i++) {
        for (var j = 0; j < map.height; j++) {
            let index = map.height * j + i;
            ctx.fillStyle = 'rgb(' + map.map[index] * 50 + ',0,0)';
            ctx.fillRect(i * widthStep, j * heightStep, widthStep, heightStep);
        }
    }

    ctx.fillStyle = 'rgb(255,255,0)';
    for (var i = 0; i < map.spawnPoints.lenght; i++) {
        let point = map.spawnPoints[i];
        ctx.fillRect(point[0] * widthStep, point[1] * heightStep, widthStep, heightStep);
    }
}