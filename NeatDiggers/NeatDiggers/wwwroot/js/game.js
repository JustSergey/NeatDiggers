import * as core from "./core.js";
import * as actions from "./actions.js";

let connection, userId;

export async function init(connect, id) {
    connection = connect;
    userId = id;

    var gameMap = await connection.invoke("GetGameMap").catch(function (err) {
        return console.error(err.toString());
    });

    core.init(gameMap);

}

let isActionInit;
export function updateRoom(room) {
    if (room.isStarted && !isActionInit) {
        actions.init()
        isActionInit = true;
    }

    core.updatePlayers(room.players, userId);
    actions.setTurn(room.players[room.playerTurn].id == userId);

    //Update Actions
    //Update UI
}

export async function doAction(gameAction) {
    let success = await connection.invoke('DoAction', gameAction).catch(function (err) {
        return console.error(err.toString());
    });
    return success;
}

export async function invoke(action) {
    let success = await connection.invoke(action).catch(function (err) {
        return console.error(err.toString());
    });
    return success;
}