import * as core from "../js/core.js";

let connection;

export async function init(connect) {
    connection = connect;

    var gameMap = await connection.invoke("GetGameMap").catch(function (err) {
        return console.error(err.toString());
    });

    core.init(gameMap);
}

export function updateRoom(room) {
    core.updatePlayers(room.players, room.userId);
    //Update Actions
    //Update UI
}

async function doAction(gameAction) {
    let success = await connection.invoke('DoAction', gameAction).catch(function (err) {
        return console.error(err.toString());
    });
    return success;
}