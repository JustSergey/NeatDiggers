

export function checkAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

export const Message = {
    NeedRollDice: "You need to roll the dice.",
    Health: "Health:",
    YouMove: "Your move!",
    ActionRemains: "Actions remains:",
    ItemLimit: "Reduce the number of items in your inventory down to 6 to complete your turn.",
    Button: {
        Drop: "Drop",
        RollDice: "Roll dice",
        Dig: "Dig",
        EndTurn: "End turn"
    }
}

export const GameActionType = {
    Move: 0,
    Dig: 1,
    Attack: 2,
    UseItem: 3,
    DropItem: 4,
    UseAbility: 5
}

//export function move(obj, pos) {
//    obj.position.set(pos.x, pos.y, pos.z);
//}