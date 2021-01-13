import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction } from './game.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();



const GameAction = {
    Move: {
        Can: true,
        Add: {
            plane: new THREE.Plane(),
            planeIntersect: new THREE.Vector3(),
            objIntersect: new THREE.Vector3(),
            shift: new THREE.Vector3(),
            isDragging: false,
            dragObject: null,
            oldPosition: new THREE.Vector3()
        },
        'take': function () {
            let intersectsPlayer = raycaster.intersectObjects(new Array(core.sPlayer));
            if (intersectsPlayer.length > 0 && this.Can) {
                this.Add.objIntersect.copy(intersectsPlayer[0].point);
                this.Add.plane.setFromNormalAndCoplanarPoint(new THREE.Vector3(0, 0, 1), this.Add.objIntersect);
                this.Add.shift.subVectors(intersectsPlayer[0].object.position, intersectsPlayer[0].point);
                this.Add.isDragging = true;
                this.Add.dragObject = intersectsPlayer[0].object;
                this.Add.oldPosition.x = intersectsPlayer[0].object.position.x;
                this.Add.oldPosition.y = intersectsPlayer[0].object.position.y;
                this.Add.oldPosition.z = intersectsPlayer[0].object.position.z;
            }
        },
        'drop': async function () {
            if (!this.Add.isDragging) return;
            this.Add.isDragging = false;

            let action = {
                targetPosition: {
                    x: this.Add.dragObject.position.x,
                    y: this.Add.dragObject.position.y
                },
                Type: 0
            }

            let success = await doAction(action);
            if (!success) core.sPlayer.position.set(this.Add.oldPosition.x, this.Add.oldPosition.y, this.Add.oldPosition.z);
            else this.Can = false;
        },
        'move': function (raycaster) {
            if (this.Add.isDragging) {
                raycaster.ray.intersectPlane(this.Add.plane, this.Add.planeIntersect);
                this.Add.dragObject.position.addVectors(this.Add.planeIntersect, this.Add.shift);
                this.Add.dragObject.position.set(Math.round(this.Add.dragObject.position.x), Math.round(this.Add.dragObject.position.y));
            }
        }
    },
    Dig: {
        Type: 1
    },
    Attack: {
        Type: 2
    },
    UseItem: {
        Type: 3
    },
    DropItem: {
        Type: 4
    },
    UseAbility: {
        Type: 5
    }
};

let State = {
    maxActionsCount: 2,
    actionsCount: 2,
}

let isMyTurn;

let camera;

export function setCamera(cam) {
    camera = cam;
}

export function init() {
    document.addEventListener('pointerup', pointerUp, false);
    document.addEventListener('pointermove', pointerMove, false);
    document.addEventListener('pointerdown', pointerDown, false);
}

export function setTurn(bool) {
    isMyTurn = bool;
}

function pointerUp(event) {
    if (!isMyTurn || State.actionsCount < 1) return;

    GameAction.Move.drop();
}

function pointerMove(event) {
    if (!isMyTurn || State.actionsCount < 1) return;

    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    GameAction.Move.move(raycaster);
}

function pointerDown(event) {
    if (!isMyTurn || State.actionsCount < 1) return;
    GameAction.Move.take();
}