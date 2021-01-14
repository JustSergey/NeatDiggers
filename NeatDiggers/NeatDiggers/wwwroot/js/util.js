import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';

export function checkAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

export const Message = {
    NeedRollDice: "You need to roll the dice",
    Health: "Health:",
    YouMove: "Your move!",
    ActionRemains: "Actions remains:",
    Button: {
        Drop: "Drop",
        RollDice: "Roll dice",
        Dig: "Dig",
        EndTurn: "End turn"
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