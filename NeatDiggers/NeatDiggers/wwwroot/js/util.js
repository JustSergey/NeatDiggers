import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';

export function checkAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

export const GameActionType = {
    Move: 0,
    Dig: 1,
    Attack: 2,
    UseItem: 3,
    DropItem: 4,
    UseAbility: 5
};

export const Cell = {
    None: 0,
    Empty: 1,
    Wall: 2,
    Digging: 3
};

export const Message = {
    NeedRollDice: "You need to roll the dice",
    Health: "Health:",
    YouMove: "Your move!",
    ActionRemains: "Actions remains:",
    Button: {
        Drop: "Drop",
        RollDice: "Roll dice",
        Dig: "Dig",
        EndTurn: "EndTurn"
    }
}

const loader = new GLTFLoader();
export function loadModel(name) {
    loader.load("../../StaticFiles/models/" + name + ".glb", function (gltf) {
        return gltf.scene.children[0];
    }, undefined, function (error) {
        console.error(error);
    });
}

//export function move(obj, pos) {
//    obj.position.set(pos.x, pos.y, pos.z);
//}