

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
        Use: "Use",
        Passive: "Is Passive Item",
        Drop: "Drop",
        RollDice: "Roll dice",
        Dig: "Dig",
        EndTurn: "End turn",
        Equip: "Equip"
    },
    ConnectionError: {
        Full: {
            Code: "full",
            Message: "The lobby is full."
        },
        Started: {
            Code: "started",
            Message: "This game is already started."
        },
        WrongCode: {
            Code: "wrongCode",
            Description: "You entered the wrong code."
        },
    }
}



export const  ItemType = {
    //Event,
    Passive: 0,
    Active: 1,
    Weapon: 2,
    Armor: 3
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