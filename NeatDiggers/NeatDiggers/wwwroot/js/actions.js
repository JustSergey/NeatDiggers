import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction, invoke } from './game.js';
import { checkAvailability, Message, GameActionType, ItemType } from './util.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();
const mouseUp = new THREE.Vector2();
const mouseDown = new THREE.Vector2();

let div, hint, turn, count, btnRollDice, btnDig, btnEndTurn, endTurnInfo, playerHealth, inventory;

const Action = {
    maxCount: 2,
    count: 2,
    diceValue: -1,
    'finishAction': function () {
        this.count--;
        turn.innerText = Message.ActionRemains + Action.count;
        count.innerText = Message.NeedRollDice;
        if (this.count < 1) {
            btnDig.disabled = true;
            btnRollDice.disabled = true;
            count.innerText = "";
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
                core.controls.enabled = false;
                if (this.Can) {
                    this.Add.objIntersect.copy(intersectsPlayer[0].point);
                    this.Add.plane.setFromNormalAndCoplanarPoint(new THREE.Vector3(0, 0, 1), this.Add.objIntersect);
                    this.Add.shift.subVectors(intersectsPlayer[0].object.position, intersectsPlayer[0].point);
                    this.Add.isDragging = true;
                    this.Add.dragObject = intersectsPlayer[0].object;
                    this.Add.oldPosition.x = intersectsPlayer[0].object.position.x;
                    this.Add.oldPosition.y = intersectsPlayer[0].object.position.y;
                    this.Add.oldPosition.z = intersectsPlayer[0].object.position.z;
                }
            }
        },
        'drop': async function () {
            if (this.Add.dragObject == null) return;
            let pos = this.Add.dragObject.position;
            let oldPos = this.Add.oldPosition;

            if (!this.Add.isDragging) return;
            this.Add.isDragging = false;

            if (pos.x == oldPos.x && pos.y == oldPos.y) {
                core.sPlayer.position.set(this.Add.oldPosition.x, this.Add.oldPosition.y, this.Add.oldPosition.z);
                return;
            }


            let action = {
                Type: GameActionType.Move,
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
    Dig: {
        Can: true,
        dig: async function () {
            let action = {
                Type: GameActionType.Dig,
                targetPosition: {
                    x: core.sPlayer.position.x,
                    y: core.sPlayer.position.y
                }
            };
            let success = await doAction(action);
            if (success) {
                btnDig.disabled = true;
                Action.Dig.Can = false;
                Action.finishAction();
            }
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

                for (var i = 0; i < players.length; i++) {
                    if (players[i].info.id == core.sPlayer.info.id) {
                        players.splice(i, 1);
                    }
                }

                let radius = await invoke("GetAttackRadius");
                if (players.length < 1) {
                    hint.style.display = 'none';
                    return;
                }
                for (var i = 0; i < players.length; i++) {
                    let btn = document.createElement('button');
                    btn.style.pointerEvents = "all";
                    if (!checkAvailability(core.sPlayer.position, players[i].info.position, radius) || !this.Can)
                        btn.disabled = true;

                    let playerId = players[i].info.id;
                    let playerName = players[i].info.name;
                    let playerHealth = players[i].info.health;
                    let playerMaxHealth = players[i].info.character.maxHealth;
                    btn.innerText = playerName + " (" + playerHealth + "/" + playerMaxHealth + ")";
                    btn.onmousedown = function () {
                        let action = {
                            Type: GameActionType.Attack,
                            TargetPlayerId: playerId
                        }
                        doAction(action);
                        hint.style.display = 'none';
                        Action.Attack.Can = false;
                        Action.finishAction();
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
        Type: GameActionType.UseItem
    },
    DropItem: {
        Type: GameActionType.DropItem
    },
    UseAbility: {
        Type: GameActionType.UseAbility
    },
    RollDise: async function () {
        Action.diceValue = await invoke('RollTheDice');
        count.innerText = Action.diceValue;
        let playerPos = core.sPlayer.info.position;
        let map = core.mapArray;
        let a1 = Action.diceValue % 2 == 0;
        let a2 = map.map[playerPos.x * map.width + playerPos.y] == 3;
        let a3 = Action.Dig.Can;
        let s = a1 && a2 && a3;
        btnDig.disabled = !(Action.diceValue % 2 == 0 && map.map[playerPos.x * map.width + playerPos.y] == 3 && Action.Dig.Can);
    },
    EndTurn: async function () {
        let success = await invoke('EndTurn');
        if (success) {
            Action.count = Action.maxCount;
            $(".ui").hide();
            Action.Move.Can = true;
            Action.Attack.Can = true;
            Action.Dig.Can = true;
            btnDig.disabled = false;
            btnRollDice.disabled = false;
        }
    },
};

let ItemsActions = {
    drop: function (item) {
        let action = {
            Type: GameActionType.DropItem,
            Item: item
        };
        doAction(action);
    },
    equip: function (inventory) {

    },
    use: function (item){
        let action = {
            Type: GameActionType.UseItem,
            Item: item
        };
        doAction(action);
    }
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

    guiInit();
}

export function updateTurn(bool, action) {
    playerHealth.innerText = Message.Health + core.sPlayer.info.health + "/" + core.sPlayer.info.character.maxHealth;
    isMyTurn = bool;
    if (bool) {
        updateInventory(core.sPlayer.info.inventory);
        if (action != null && action.type == GameActionType.DropItem) return;
        turn.innerText = Action.count < 1 ? Message.YouMove : Message.ActionRemains + Action.count;
        count.innerText = Message.NeedRollDice;
        $(".ui").show();
        div.style.display = 'block';
        btnDig.disabled = !(Action.diceValue % 2 == 0 && map.map[playerPos.x * map.width + playerPos.y] == 3 && Action.Dig.Can);
    }
}

function updateInventory(inv) {
    updateItems(inv.items);

    if (core.sPlayer.info.inventory.items.length > 6) {
        btnEndTurn.disabled = true;
        endTurnInfo.style.display = "block";
    }
    else {
        btnEndTurn.disabled = false;
        endTurnInfo.style.display = "none";
    }
}

function updateItems(items) {
    while (inventory.firstChild) {
        inventory.removeChild(inventory.firstChild);
    }

    for (var i = 0; i < items.length; i++) {
        let item = items[i];
        let itemUse = document.createElement("button");
        let itemDrop = document.createElement("button");
        let itemDescription = document.createElement("p");

        itemUse.style.pointerEvents = "all";
        itemUse.classList.add("ui");
        itemDrop.innerText = Message.Button.Drop;
        itemDrop.style.pointerEvents = "all";
        itemDrop.classList.add("ui");
        itemDrop.onclick = function () { ItemsActions.drop(item); };
        itemDescription.innerText = item.title + " (" + item.description + ")";

        switch (item.type) {
            case ItemType.Active:
                itemUse.innerText = Message.Button.Use;
                itemUse.onclick = function () { ItemsActions.use(item); };
                break;
            case ItemType.Passive:
                itemUse.innerText = Message.Button.Passive;
                itemUse.disabled = true;
                break;
            case ItemType.Armor:
                itemUse.innerText = Message.Button.Equip;
                itemUse.disabled = true;
                break;
            case ItemType.Weapon:
                itemUse.innerText = Message.Button.Equip;
                itemUse.disabled = true;
                break;
            default:
        }

        inventory.appendChild(itemDescription);
        inventory.appendChild(itemUse);
        inventory.appendChild(itemDrop);
    }
}

function guiInit() {
    div = document.createElement("div");
    div.style.position = 'absolute';
    div.style.pointerEvents = "none";
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
    btnRollDice.style.pointerEvents = "all";
    btnRollDice.classList.add("ui");
    btnRollDice.innerText = Message.Button.RollDice;
    btnRollDice.onclick = Action.RollDise;
    btnRollDice.onselectstart = false;
    btnRollDice.onmousedown = false;
    div.appendChild(btnRollDice);

    btnDig = document.createElement("button");
    btnDig.style.pointerEvents = "all";
    btnDig.classList.add("ui");
    btnDig.innerText = Message.Button.Dig;
    btnDig.onclick = Action.Dig.dig;
    btnDig.onselectstart = false;
    btnDig.onmousedown = false;
    btnDig.disabled = true;
    div.appendChild(btnDig);

    endTurnInfo = document.createElement("P");
    endTurnInfo.style.color = "white";
    endTurnInfo.innerText = Message.ItemLimit;
    endTurnInfo.style.display = "none";
    endTurnInfo.onselectstart = false;
    endTurnInfo.onmousedown = false;
    div.appendChild(endTurnInfo);

    btnEndTurn = document.createElement("button");
    btnEndTurn.style.pointerEvents = "all";
    btnEndTurn.classList.add("ui");
    btnEndTurn.innerText = Message.Button.EndTurn;
    btnEndTurn.onclick = Action.EndTurn;
    btnEndTurn.onselectstart = false;
    btnEndTurn.onmousedown = false;
    div.appendChild(btnEndTurn);

    inventory = document.createElement("div");
    inventory.style.color = "white";
    inventory.onselectstart = false;
    inventory.onmousedown = false;
    div.appendChild(inventory);

    $(".ui").hide();
}

function pointerUp(event) {
    core.controls.enabled = true;
    mouseUp.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouseUp.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouseUp, camera);
    Action.Attack.showHint(raycaster, event.clientX, event.clientY);

    if (!isMyTurn || Action.count < 1) return;
    Action.Move.drop();
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