import * as core from "./core.js";
import * as actions from "./actions.js";

let connection, userId;


export async function init(connect, id) {
    connection = connect;
    userId = id;

    var gameMap = await connection.invoke("GetGameMap").catch(function (err) {
        return console.error(err.toString());
    });

    await core.init(gameMap);
}

let isActionInit;
export async function updateRoom(room, action) {
    if (room.isStarted && !isActionInit) {
        await actions.init()
        isActionInit = true;
    }

    core.updatePlayers(room.players, userId);
    core.UpdateFlag(getFlagPosition(room), room.flagOnTheGround);
    actions.updateTurn(room.players[room.playerTurn].id == userId, action);
}

function getFlagPosition(room) {
    if (!room.flagOnTheGround)
        for (var i = 0; i < room.players.length; i++)
            if (room.players[i].withFlag)
                return room.players[i].position;
    return room.flagPosition
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