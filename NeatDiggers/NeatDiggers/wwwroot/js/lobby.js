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
    document.getElementById("spectators").innerText = room.spectators.length;
    DrawMap(document.getElementById("map"), room.gameMap);
}

connection.on("ChangeState", function (room) {
    ChangeState(room);
});

connection.on("ChangeStateWithAction", function (room, gameAction) {
    ChangeState(room);
});

function DrawMap(canvas, map) {
    let ctx = canvas.getContext("2d");
    var widthStep = canvas.width / map.width;
    var heightStep = canvas.height / map.height;
    ctx.font = '48px serif';
    ctx.fillRect(0, 0, map.height, map.width);
    for (var i = 0; i < map.width; i++) {
        for (var j = 0; j < map.height; j++) {
            let index = map.height * j + i;
            ctx.fillStyle = 'rgb(' + map.map[index] * 50 +',0,0)'; 
            ctx.fillRect(i * widthStep, j * heightStep, widthStep, heightStep);
        }
    }

    ctx.fillStyle = 'rgb(255,255,0)'; 
    for (var i = 0; i < map.spawnPoints.lenght; i++) {
        let point = map.spawnPoints[i];
        ctx.fillRect(point[0] * widthStep, point[1] * heightStep, widthStep, heightStep);
    }
}