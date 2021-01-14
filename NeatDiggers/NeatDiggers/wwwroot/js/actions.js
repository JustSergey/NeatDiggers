import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction, invoke } from './game.js';
import { checkAvailability, Message } from './util.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();

let div, hint, turn, count, btnRollDice, btnDig, btnEndTurn, playerHealth;

const Action = {
    maxCount: 2,
    count: 2,
    'finishAction': function () {
        this.count--;
        turn.innerText = Message.ActionRemains + Action.count;
        count.innerText = Message.NeedRollDice;
        if (this.count < 1) {
            btnDig.disabled = true;
            btnRollDice.disabled = true;
        }
    },
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
                Type: 0,
                targetPosition: {
                    x: this.Add.dragObject.position.x,
                    y: this.Add.dragObject.position.y
                }
            }

            let success = await doAction(action);
            if (success) {
                this.Can = false;
                Action.finishAction();
            }
            else
                core.sPlayer.position.set(this.Add.oldPosition.x, this.Add.oldPosition.y, this.Add.oldPosition.z);
        },
        'move': function (raycaster) {
            if (this.Add.isDragging) {
                raycaster.ray.intersectPlane(this.Add.plane, this.Add.planeIntersect);
                this.Add.dragObject.position.addVectors(this.Add.planeIntersect, this.Add.shift);
                this.Add.dragObject.position.set(Math.round(this.Add.dragObject.position.x), Math.round(this.Add.dragObject.position.y));
            }
        }
    },
    Dig: async function () {
        let action = {
            Type: 1,
            targetPosition: {
                x: core.sPlayer.position.x,
                y: core.sPlayer.position.y
            }
        };
        let success = await doAction(action);
        if (success) {
            btnDig.disabled = true;
            Action.finishAction();
        }
    },
    Attack: {
        Can: true,
        'showHint': async function (raycaster, x, y) {
            let intersectsPlayers = raycaster.intersectObjects(core.sPlayers.children);
            if (intersectsPlayers.length > 0) {
                while (hint.firstChild) {
                    hint.removeChild(hint.firstChild);
                }
                let radius = await invoke("GetAttackRadius");
                for (var i = 0; i < intersectsPlayers.length; i++) {
                    let btn = document.createElement('button');
                    if (!checkAvailability(core.sPlayer.position, intersectsPlayers[i].object.position, radius) && !this.Can)
                        btn.disabled = true;

                    let playerId = intersectsPlayers[i].object.info.id;
                    let playerName = intersectsPlayers[i].object.info.name;
                    let playerHealth = intersectsPlayers[i].object.info.health;
                    let playerMaxHealth = intersectsPlayers[i].object.info.character.maxHealth;
                    btn.innerText = playerName + " (" + playerHealth + "/" + playerMaxHealth + ")";
                    btn.onclick = function () {
                        let action = {
                            Type: 2,
                            TargetPlayerId: playerId
                        }
                        doAction(action);
                        hint.style.display = 'none';
                    }
                    hint.appendChild(btn);
                    hint.style.display = 'block';
                }
                hint.style.left = x + "px";
                hint.style.top = y + "px";
            }
            else
                hint.style.display = 'none';
        },
    },
    UseItem: {
        Type: 3
    },
    DropItem: {
        Type: 4
    },
    UseAbility: {
        Type: 5
    },
    RollDise: async function () {
        let result = await invoke('RollTheDice');
        count.innerText = result;
        btnDig.disabled = !(result % 2 == 0);
    },
    EndTurn: async function () {
        let success = await invoke('EndTurn');
        if (success) {
            Action.count = Action.maxCount;
            div.style.display = 'none';
        }
    },
};

let isMyTurn;
let camera;

export function setCamera(cam) {
    camera = cam;
}

export function init() {
    document.addEventListener('pointerup', pointerUp, false);
    document.addEventListener('pointermove', pointerMove, false);
    document.addEventListener('pointerdown', pointerDown, false);

    guiInit();
}

export function setTurn(bool) {
    isMyTurn = bool;
    if (bool) {
        Action.Move.Can = true;
        turn.innerText = Message.YouMove;
        count.innerText = Message.NeedRollDice;
        div.style.display = 'block';

        btnDig.disabled = false;
        btnRollDice.disabled = false;
    }

    playerHealth.innerText = Message.Health + core.sPlayer.info.health + "/" + core.sPlayer.info.character.maxHealth;
}

function guiInit() {
    div = document.createElement("div");
    div.style.position = 'absolute';
    div.style.display = 'none';
    div.style.border = '1px solid black';
    div.style.width = core.screen.width + 'px';
    div.style.height = core.screen.height + 'px';
    div.style.margin = 'auto';
    div.style.top = $('header').outerHeight() + 'px';
    div.onselectstart = false;
    div.onmousedown = false;
    document.body.appendChild(div);

    hint = document.createElement("div");
    hint.id = "hint";
    div.appendChild(hint);

    playerHealth = document.createElement("p");
    playerHealth.style.color = "white";
    div.appendChild(playerHealth);

    turn = document.createElement("p");
    turn.style.color = "white";
    div.appendChild(turn);

    count = document.createElement("P");
    count.style.color = "white";
    count.innerText = Message.NeedRollDice;
    count.onselectstart = false;
    count.onmousedown = false;
    div.appendChild(count);

    btnRollDice = document.createElement("button");
    btnRollDice.innerText = Message.Button.RollDice;
    btnRollDice.onclick = Action.RollDise;
    btnRollDice.onselectstart = false;
    btnRollDice.onmousedown = false;
    div.appendChild(btnRollDice);

    btnDig = document.createElement("button");
    btnDig.innerText = Message.Button.Dig;
    btnDig.onclick = Action.Dig;
    btnDig.onselectstart = false;
    btnDig.onmousedown = false;
    btnDig.disabled = true;
    div.appendChild(btnDig);

    btnEndTurn = document.createElement("button");
    btnEndTurn.innerText = Message.Button.EndTurn;
    btnEndTurn.onclick = Action.EndTurn;
    btnEndTurn.onselectstart = false;
    btnEndTurn.onmousedown = false;
    div.appendChild(btnEndTurn);
}

function pointerUp(event) {
    if (!isMyTurn || Action.count < 1) return;

    Action.Move.drop();
    Action.Attack.showHint(raycaster, event.clientX, event.clientY);
}

function pointerMove(event) {
    if (!isMyTurn || Action.count < 1) return;

    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    Action.Move.move(raycaster);
}

function pointerDown(event) {
    if (!isMyTurn || Action.count < 1) return;

    Action.Move.take();
}