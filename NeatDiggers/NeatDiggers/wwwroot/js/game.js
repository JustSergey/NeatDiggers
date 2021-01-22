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
    let flagPosition = getFlagPosition(room);
    core.UpdateFlag(flagPosition, room.flagOnTheGround);
    let playerPosition = room.players[room.playerTurn].position;
    actions.ShowTakeFlagButton(room.flagOnTheGround && isEqualVector(playerPosition, flagPosition));
    actions.updateTurn(room.players[room.playerTurn].id == userId, action);
    CheckWiner(room.winner);
}

function CheckWiner(winner) {
    if (winner != null) {
        $("#errorModal").modal();
        $("#errorModalMessage").text("The winner is determined!");
        $("#errorModalMessage").text(winner.name + " (" + winner.character.name + ")");
        $("#errorModalFooter").hide();
    }
}

function isEqualVector(vec1, vec2) {
    return vec1.x == vec2.x && vec1.y == vec2.y;
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

export async function ChangeInventory(inventory) {
    let success = await connection.invoke('ChangeInventory', inventory).catch(function (err) {
        return console.error(err.toString());
    });
    return success;
}