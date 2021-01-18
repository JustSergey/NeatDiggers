import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';
const loader = new GLTFLoader();

export function checkAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

export async function modelLoader(name) {
    return new Promise((resolve, reject) => {
        loader.load("../../StaticFiles/models/" + name + ".glb", data => resolve(data.scene.children[0]), null, reject);
    });
}

export const Message = {
    NeedRollDice: "You need to roll the dice.",
    Health: "Health:",
    YouMove: "Your move!",
    ActionRemains: "Actions remains:",
    ItemLimit: "Reduce the number of items in your inventory down to 6 to complete your turn.",
    Button: {
        Drop: "Drop",
        Passive: "Is Passive Item",
        Use: {
            None: "Use",
            Player: "Use on player",
            Position: "Use on position",
        },
        Equip: {
            Armor: "Equip",
            Left: "Left",
            Right: "Right",
            Two: "Two-handed",
        },
        RollDice: "Roll dice",
        Dig: "Dig",
        EndTurn: "End turn"
    }
}

export const Conection = {
    Error: {
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

export const Target = {
    None: 0,
    Player: 1,
    Position: 2
}

export const ItemType = {
    //Event,
    Passive: 0,
    Active: 1,
    Weapon: 2,
    Armor: 3
}

export const WeaponHanded = {
    None: 0,
    One: 1,
    Two: 2
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