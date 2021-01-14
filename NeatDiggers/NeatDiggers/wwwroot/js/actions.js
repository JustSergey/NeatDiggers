import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction, invoke } from './game.js';
import { checkAvailability, Message } from './util.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();
const mouseUp = new THREE.Vector2();
const mouseDown = new THREE.Vector2();

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
            let intersectsObjects = raycaster.intersectObjects(core.scene.children);
            if (intersectsObjects.length > 0) {
                while (hint.firstChild) {
                    hint.removeChild(hint.firstChild);
                }

                let players = new Array();
                let position = intersectsObjects[0].object.position;
                for (var i = 0; i < core.sPlayers.children.length; i++) {
                    let player = core.sPlayers.children[i];
                    if (player.info.position.x == position.x && player.info.position.y == position.y) {
                        players.push(player);
                    }
                }

                let radius = await invoke("GetAttackRadius");
                if (players.length < 1) {
                    hint.style.display = 'none';
                    return;
                }
                for (var i = 0; i < players.length; i++) {
                    let btn = document.createElement('button');
                    if (!checkAvailability(core.sPlayer.position, players[i].info.position, radius) || !this.Can)
                        btn.disabled = true;

                    let playerId = players[i].info.id;
                    let playerName = players[i].info.name;
                    let playerHealth = players[i].info.health;
                    let playerMaxHealth = players[i].info.character.maxHealth;
                    btn.innerText = playerName + " (" + playerHealth + "/" + playerMaxHealth + ")";
                    btn.onmousedown = function () {
                        let action = {
                            Type: 2,
                            TargetPlayerId: playerId
                        }
                        doAction(action);
                        hint.style.display = 'none';
                        Action.Attack.Can = false;
                    }
                    hint.appendChild(btn);
                    hint.style.display = 'block';
                }
                hint.style.left = x + "px";
                hint.style.top = y + "px";
            }
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
        let playerPos = core.sPlayer.info.position;
        let map = core.mapArray;
        btnDig.disabled = !(result % 2 == 0) || !(map.map[playerPos.x * map.width + playerPos.y] == 3);
    },
    EndTurn: async function () {
        let success = await invoke('EndTurn');
        if (success) {
            Action.count = Action.maxCount;
            $(".ui").hide();
            Action.Move.Can = true;
            Action.Attack.Can = true;
            btnDig.disabled = false;
            btnRollDice.disabled = false;
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
        turn.innerText = Message.YouMove;
        count.innerText = Message.NeedRollDice;
        $(".ui").show();
        div.style.display = 'block';
    }

    playerHealth.innerText = Message.Health + core.sPlayer.info.health + "/" + core.sPlayer.info.character.maxHealth;
}

function guiInit() {
    div = document.createElement("div");
    div.style.position = 'absolute';
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
    turn.classList.add("ui");
    div.appendChild(turn);

    count = document.createElement("P");
    count.style.color = "white";
    count.classList.add("ui");
    count.innerText = Message.NeedRollDice;
    count.onselectstart = false;
    count.onmousedown = false;
    div.appendChild(count);

    btnRollDice = document.createElement("button");
    btnRollDice.classList.add("ui");
    btnRollDice.innerText = Message.Button.RollDice;
    btnRollDice.onclick = Action.RollDise;
    btnRollDice.onselectstart = false;
    btnRollDice.onmousedown = false;
    div.appendChild(btnRollDice);

    btnDig = document.createElement("button");
    btnDig.classList.add("ui");
    btnDig.innerText = Message.Button.Dig;
    btnDig.onclick = Action.Dig;
    btnDig.onselectstart = false;
    btnDig.onmousedown = false;
    btnDig.disabled = true;
    div.appendChild(btnDig);

    btnEndTurn = document.createElement("button");
    btnEndTurn.classList.add("ui");
    btnEndTurn.innerText = Message.Button.EndTurn;
    btnEndTurn.onclick = Action.EndTurn;
    btnEndTurn.onselectstart = false;
    btnEndTurn.onmousedown = false;
    div.appendChild(btnEndTurn);

    $(".ui").hide();
}

function pointerUp(event) {
    if (!isMyTurn || Action.count < 1) return;

    mouseUp.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouseUp.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;

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

    mouseDown.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouseDown.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;

    Action.Move.take();
}