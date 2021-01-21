import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction, invoke, ChangeInventory } from './game.js';
import { checkAvailability, Message, GameActionType, ItemType, Target, WeaponHanded, modelLoader, WeaponType, getKeyByValue, Ability } from './util.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();

let ui = {
    div: document.createElement("div"),
    hint: {
        container: document.createElement("div"),
        clear: function () { ui.clear(this.container); },
        setPosition: function (x, y) {
            this.container.style.left = x + "px";
            this.container.style.top = y + "px";
        },
        hide: function () { this.container.style.display = 'none'; },
        show: function () { this.container.style.display = 'block'; }
    },
    message: {
        turn: document.createElement("p"),
        error: document.createElement("p"),
        actionCount: document.createElement("p"),
        init: function () {
            this.turn.classList.add("ui");
            this.actionCount.classList.add("ui");
            this.error.classList.add("ui");

            this.error.style.color = "red";

            ui.div.appendChild(this.turn);
            ui.div.appendChild(this.actionCount);
            ui.div.appendChild(this.error);
        },
        update: function (action) {
            if (action != null && action.type == GameActionType.DropItem) return;
            Action.finishAction();
        }
    },
    button: {
        dig: document.createElement("button"),
        rollDice: document.createElement("button"),
        takeFlag: document.createElement("button"),
        end: document.createElement("button"),
        init: function () {
            this.dig.style.pointerEvents = "all";
            this.rollDice.style.pointerEvents = "all";
            this.takeFlag.style.pointerEvents = "all";
            this.end.style.pointerEvents = "all";

            this.dig.classList.add("ui");
            this.rollDice.classList.add("ui");
            this.takeFlag.classList.add("ui");
            this.end.classList.add("ui");

            this.dig.disabled = true;
            this.takeFlag.disabled = true;

            this.dig.innerText = Message.Button.Dig;
            this.rollDice.innerText = Message.Button.RollDice;
            this.takeFlag.innerText = Message.Button.TakeFlag;
            this.end.innerText = Message.Button.EndTurn;

            this.dig.onclick = Action.Dig.dig;
            this.rollDice.onclick = Action.RollDise;
            this.takeFlag.onclick = Action.TakeFlag;
            this.end.onclick = Action.EndTurn;

            ui.div.appendChild(this.dig);
            ui.div.appendChild(this.rollDice);
            ui.div.appendChild(this.takeFlag);
            ui.div.appendChild(this.end);
        },
        update: function () {
            this.rollDice.disabled = Action.count < 1;
        }
    },
    player: {
        container: document.createElement("div"),
        name: document.createElement("p"),
        health: document.createElement("p"),
        level: document.createElement("p"),
        score: document.createElement("p"),
        weaponType: document.createElement("p"),
        abilities: {
            container: document.createElement("div"),
            title: document.createElement("p"),
            init: function () {
                this.title.innerText = Message.Abilities;
                this.title.classList.add("ui");
                this.container.classList.add("abilities");
                ui.player.container.appendChild(this.title);
                ui.player.container.appendChild(this.container);
            },
            update: function (abilities) {
                this.clear();
                for (var i = 0; i < abilities.length; i++) {
                    if (abilities[i].isActive) {
                        let description = document.createElement("p");
                        description.innerText = getKeyByValue(Ability.Name, abilities[i].name) + " (" + abilities[i].description + ")";
                        this.container.appendChild(description);
                    }
                }
            },
            clear: function () { ui.clear(this.container); }
        },
        inventory: {
            container: document.createElement("div"),
            leftWeapon: document.createElement("p"),
            rightWeapon: document.createElement("p"),
            armor: document.createElement("p"),
            drop: document.createElement("p"),
            items: {
                container: document.createElement("div"),
                title: document.createElement("p"),
                init: function () {
                    this.title.innerText = Message.Inventory.Title;
                    this.title.classList.add("ui");
                    this.container.classList.add("items");
                    ui.player.inventory.container.appendChild(this.title);
                    ui.player.inventory.container.appendChild(this.container);
                },
                update: function (items) {
                    this.clear();

                    for (var i = 0; i < items.length; i++) {
                        let item = items[i];
                        let itemUse = document.createElement("button");
                        let itemDrop = document.createElement("button");
                        let itemDescription = document.createElement("p");
                        let itemRight = null;

                        itemUse.style.pointerEvents = "all";
                        itemUse.classList.add("ui");
                        switch (item.type) {
                            case ItemType.Active:
                                itemUse.classList.add("itemButton");
                                switch (item.target) {
                                    case Target.None:
                                        itemUse.innerText = Message.Button.Use.None;
                                        itemUse.onclick = function () { ItemsActions.use(item); };
                                        break;
                                    case Target.Player:
                                        itemUse.innerText = Message.Button.Use.Player;
                                        itemUse.onclick = function () { ItemsActions.onPlayer.init(item); };
                                        break;
                                    case Target.Position:
                                        itemUse.innerText = Message.Button.Use.Position;
                                        itemUse.onclick = function () { ItemsActions.onPosition.init(item); };
                                        break;
                                    default:
                                }
                                break;
                            case ItemType.Passive:
                                itemUse.innerText = Message.Button.Passive;
                                itemUse.disabled = true;
                                break;
                            case ItemType.Armor:
                                itemUse.innerText = Message.Button.Equip.Armor;
                                itemUse.onclick = function () { ItemsActions.equipArmor(item); };
                                break;
                            case ItemType.Weapon:
                                switch (item.weaponHanded) {
                                    case WeaponHanded.One:
                                        itemRight = document.createElement("button");
                                        itemRight.style.pointerEvents = "all";
                                        itemUse.innerText = Message.Button.Equip.Left;
                                        itemRight.innerText = Message.Button.Equip.Right;
                                        itemUse.onclick = function () { ItemsActions.equipLeft(item); };
                                        itemRight.onclick = function () { ItemsActions.equipRight(item); };
                                        break;
                                    case WeaponHanded.Two:
                                        itemUse.innerText = Message.Button.Equip.Two;
                                        itemUse.onclick = function () { ItemsActions.equipTwo(item); };
                                        break;
                                    default:
                                }
                                break;
                        }
                        itemDrop.style.pointerEvents = "all";
                        itemDrop.classList.add("ui");
                        itemDrop.innerText = Message.Button.Drop;
                        itemDrop.onclick = function () { ItemsActions.drop(item); };

                        itemDescription.innerText = item.title + " (" + item.description + ")";

                        this.container.appendChild(itemDescription);
                        this.container.appendChild(itemUse);
                        if (itemRight != null) this.container.appendChild(itemRight);
                        this.container.appendChild(itemDrop);
                    }
                },
                clear: function () { ui.clear(this.container); }
            },
            init: function () {
                this.container.classList.add("inventory");
                ui.player.container.appendChild(this.container);
                this.container.appendChild(this.leftWeapon);
                this.container.appendChild(this.rightWeapon);
                this.container.appendChild(this.armor);
                this.container.appendChild(this.drop);
                this.items.init();
            },
            update: function (inventory) {
                this.leftWeapon.innerText = Message.Inventory.LeftWeapon;
                this.armor.innerText = Message.Inventory.Armor;
                this.rightWeapon.innerText = Message.Inventory.RightWeapon;


                if (inventory.rightWeapon.title != null)
                    this.rightWeapon.innerText += inventory.rightWeapon.title + " (" + inventory.rightWeapon.description + ")";
                else if (inventory.leftWeapon.weaponHanded == WeaponHanded.Two) {
                    this.leftWeapon.innerText = Message.Inventory.Two;
                    this.rightWeapon.innerText = "";
                }
                if (inventory.leftWeapon.title != null)
                    this.leftWeapon.innerText += inventory.leftWeapon.title + " (" + inventory.leftWeapon.description + ")";

                if (inventory.armor.title != null)
                    this.armor.innerText += inventory.armor.title + " (" + inventory.armor.description + ")";

                this.drop.innerText = Message.Inventory.Drop + inventory.drop;
                this.items.update(inventory.items);
            }
        },
        effects: {
            container: document.createElement("div"),
            title: document.createElement("p"),
            init: function () {
                this.title.innerText = Message.Effects;
                this.title.classList.add("ui");
                this.container.classList.add("effects");
                ui.player.inventory.container.appendChild(this.title);
                ui.player.inventory.container.appendChild(this.container);
            },
            update: function (effects) {
                this.clear();
                for (var i = 0; i < effects.length; i++) {
                    let description = document.createElement("p");
                    description.innerText = effects[i].title;
                    this.container.appendChild(description);
                }
            },
            clear: function () { ui.clear(this.container); }
        },
        init: function () {
            this.container.classList.add("player");
            this.container.style.pointerEvents = "none";
            this.container.style.color = "white";
            ui.div.appendChild(this.container);

            this.container.appendChild(this.name);
            this.container.appendChild(this.health);
            this.container.appendChild(this.level);
            this.container.appendChild(this.score);
            this.container.appendChild(this.weaponType);
            this.abilities.init();
            this.inventory.init();
            this.effects.init();
        },
        update: function (player) {
            this.name.innerText = player.name + " (" + player.character.title + ")";
            this.health.innerText = Message.Health + player.health + "/" + player.character.maxHealth;
            this.level.innerText = Message.Level + player.level;
            this.score.innerText = Message.Score + player.score;
            this.weaponType.innerText = Message.WeaponType + getKeyByValue(WeaponType, player.character.weaponType);

            this.abilities.update(player.character.abilities);
            this.inventory.update(player.inventory);
            this.effects.update(player.effects);
        }
    },
    init: function () {
        this.div.style.position = 'absolute';
        this.div.style.pointerEvents = "none";
        this.div.style.width = core.screen.width + 'px';
        this.div.style.height = core.screen.height + 'px';
        this.div.style.margin = 'auto';
        this.div.style.color = "white";
        this.div.style.top = $('header').outerHeight() + 'px';
        document.body.appendChild(this.div);

        this.hint.container.id = "hint";
        this.div.appendChild(this.hint.container);

        this.player.init();
        this.message.init();
        this.button.init();
        $(".ui").hide();
    },
    update: function (player, action, isMyTurn) {
        this.player.update(player);
        this.message.update(action);
        this.button.update();

        if (isMyTurn)
            $(".ui").show();
        else
            $(".ui").hide();
    },
    clear: function (container) {
        while (container.firstChild)
            container.removeChild(container.firstChild);
    }
}
let target;

export function ShowTakeFlagButton(isActive) {
    ui.button.takeFlag.disabled = !isActive;
}

function isPlayerCanDig() {
    let playerPos = core.sPlayer.info.position;
    let map = core.mapArray;
    return Action.diceValue % 2 == 0 && map.map[playerPos.x * map.width + playerPos.y] == 3 && Action.Dig.Can
}

const Action = {
    maxCount: 2,
    count: 2,
    diceValue: -1,
    finishAction: function () {
        ui.message.turn.innerText = Message.ActionRemains + Action.count;
        Action.diceValue = -1;
        ui.message.actionCount.innerText = Message.NeedRollDice;
        ui.button.dig.disabled = !isPlayerCanDig();
        if (this.count < 1) {
            ui.button.dig.disabled = true;
            ui.button.rollDice.disabled = true;
            ui.button.takeFlag.disabled = true;
            ui.message.actionCount.innerText = "";
            $(".itemButton").prop("disabled", true);
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
        take: function (player) {
            if (player != null && this.Can) {
                core.controls.enabled = false;
                if (this.Can) {
                    this.Add.objIntersect.copy(player.point);
                    this.Add.plane.setFromNormalAndCoplanarPoint(new THREE.Vector3(0, 0, 1), this.Add.objIntersect);
                    this.Add.shift.subVectors(player.position, player.point);
                    this.Add.isDragging = true;
                    this.Add.dragObject = player;
                    this.Add.oldPosition.x = player.position.x;
                    this.Add.oldPosition.y = player.position.y;
                    this.Add.oldPosition.z = player.position.z;
                }
            }
        },
        drop: async function () {
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
                Action.count--;
                Action.finishAction();
            }
            else
                core.sPlayer.position.set(this.Add.oldPosition.x, this.Add.oldPosition.y, this.Add.oldPosition.z);
        },
        move: function (raycaster) {
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
                ui.button.dig.disabled = true;
                Action.Dig.Can = false;
                Action.count--;
                Action.finishAction();
            }
        }
    },
    Attack: {
        Can: true,
        showHint: async function (players) {
            let radius = await invoke("GetAttackRadius");

            for (var i = 0; i < players.length; i++) {
                let btn = document.createElement('button');
                btn.style.pointerEvents = "all";
                if (!(checkAvailability(core.sPlayer.position, players[i].info.position, radius) && this.Can && Action.count > 0))
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
                    let success = doAction(action);
                    ui.hint.hide();
                    if (success) {
                        Action.Attack.Can = false;
                        Action.count--;
                        Action.finishAction();
                    }
                }
                ui.hint.container.appendChild(btn);
                ui.hint.show();
            }
        },
    },
    RollDise: async function () {
        Action.diceValue = await invoke('RollTheDice');
        ui.message.actionCount.innerText = Action.diceValue;
        ui.button.dig.disabled = !isPlayerCanDig();
    },
    EndTurn: async function () {
        let success = await invoke('EndTurn');
        if (success) {
            $(".ui").hide();
            Action.count = Action.maxCount;
            Action.diceValue = -1;
            Action.Move.Can = true;
            Action.Attack.Can = true;
            Action.Dig.Can = true;
            target.visible = false;
            ItemsActions.onPlayer.listen = false;
            ItemsActions.onPosition.listen = false;
            ui.button.dig.disabled = true;
        }
    },
    TakeFlag: async function () {
        let action = {
            Type: GameActionType.TakeTheFlag
        };
        let success = await doAction(action);
        if (success) {
            ui.button.takeFlag.disabled = true;
            Action.count--;
            Action.finishAction();
        }
    }
};

function getPlayers(raycaster) {
    let players = {
        user: null,
        other: new Array()
    }
    let intersectsObjects = raycaster.intersectObjects(core.scene.children);
    if (intersectsObjects.length > 0) {
        let position = intersectsObjects[0].object.position;
        players.point = intersectsObjects[0].point
        for (var i = 0; i < core.sPlayers.children.length; i++) {
            let player = core.sPlayers.children[i];
            if (player.info.position.x == position.x && player.info.position.y == position.y)
                players.other.push(player);
        }

        for (var i = 0; i < players.other.length; i++) {
            if (players.other[i].info.id == core.sPlayer.info.id) {
                players.user = players.other[i];
                players.user.point = intersectsObjects[0].point;
                players.other.splice(i, 1);
            }
        }
    }
    return players;
}

function arrayRemove(arr, value) {
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].name == value.name) {
            arr.splice(i, 1);
            return;
        }
    }
}

let ItemsActions = {
    drop: function (item) {
        let action = {
            Type: GameActionType.DropItem,
            Item: item
        };
        doAction(action);
    },
    equipArmor: async function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.armor.title != null)
            inventory.items.push(inventory.armor);
        inventory.armor = item;
        arrayRemove(inventory.items, item);

        let success = await ChangeInventory(inventory);
    },
    equipLeft: async function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.leftWeapon.title != null)
            inventory.items.push(inventory.leftWeapon);
        inventory.leftWeapon = item;
        arrayRemove(inventory.items, item);

        let success = await ChangeInventory(inventory);
    },
    equipRight: function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.rightWeapon.title != null)
            inventory.items.push(inventory.rightWeapon);
        inventory.rightWeapon = item;
        arrayRemove(inventory.items, item);

        ChangeInventory(inventory);
    },
    equipTwo: function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.leftWeapon.title != null)
            inventory.items.push(inventory.leftWeapon);
        if (inventory.rightWeapon.title != null)
            inventory.items.push(inventory.rightWeapon);
        inventory.leftWeapon = item;
        arrayRemove(inventory.items, item);

        ChangeInventory(inventory);
    },
    use: async function (item) {
        let action = {
            Type: GameActionType.UseItem,
            Item: item
        };
        let success = await doAction(action);
        if (success) {
            Action.count--;
            Action.finishAction();
        }
    },
    onPlayer: {
        listen: false,
        item: null,
        init: function (item) {
            this.item = item;
            this.listen = true;
            target.visible = true;
        },
        showHint: function (players) {
            if (!this.listen || Action.count < 1) return;
            target.visible = false;
            for (var i = 0; i < players.length; i++) {
                let btn = document.createElement('button');
                btn.style.pointerEvents = "all";

                let playerId = players[i].info.id;
                let playerName = players[i].info.name;
                btn.innerText = "Use on: " + playerName;
                btn.onmousedown = async function () {
                    let action = {
                        Type: GameActionType.UseItem,
                        TargetPlayerId: playerId,
                        Item: ItemsActions.onPlayer.item
                    }
                    let success = await doAction(action);

                    if (success) {
                        Action.count--;
                        Action.finishAction();
                    }

                    ui.hint.hide();
                    this.listen = false;
                    this.item = null;
                    target.position.set();
                }
                ui.hint.container.appendChild(btn);
                ui.hint.show();
            }
        }
    },
    onPosition: {
        listen: false,
        item: null,
        init: function (item) {
            this.item = item;
            this.listen = true;
            target.visible = true;
            target.position.set();
        },
        use: async function () {
            if (!this.listen || action.count < 1) return;
            let action = {
                Type: GameActionType.UseItem,
                TargetPosition: target.position,
                Item: this.item
            };
            let success = await doAction(action);
            target.visible = false;
            if (success) {
                Action.count--;
                Action.finishAction();
            }

            this.listen = false;
            this.item = null;
            target.position.set();
        }
    }
}

let isMyTurn;
let camera;

export function setCamera(cam) {
    camera = cam;
}

export async function init() {
    target = await modelLoader("target");
    target.position.set();
    target.visible = false;
    core.scene.add(target);
    document.addEventListener('pointerup', pointerUp, false);
    document.addEventListener('pointermove', pointerMove, false);
    document.addEventListener('pointerdown', pointerDown, false);

    ui.init();
}

export function updateTurn(bool, action) {
    ui.update(core.sPlayer.info, action, bool);
    isMyTurn = bool;
}

function pointerUp(event) {
    core.controls.enabled = true;
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    let players = getPlayers(raycaster).other;
    if (players.length > 0) {
        ui.hint.clear();
        ui.hint.setPosition(event.clientX, event.clientY);

        Action.Attack.showHint(players);
        ItemsActions.onPlayer.showHint(players);
    }
    else
        ui.hint.hide();

    if (!isMyTurn || Action.count < 1) return;
    Action.Move.drop();
    ItemsActions.onPosition.use();
}

function pointerMove(event) {
    if (!isMyTurn || Action.count < 1) return;

    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    Action.Move.move(raycaster);

    if (target.visible) {
        let intersectsObjects = raycaster.intersectObjects(core.scene.children);
        if (intersectsObjects.length > 0)
            target.position.set(intersectsObjects[0].object.position.x, intersectsObjects[0].object.position.y, 0);
        else
            target.position.set();
    }
}

function pointerDown(event) {
    if (!isMyTurn || Action.count < 1) return;

    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;

    raycaster.setFromCamera(mouse, camera);

    let player = getPlayers(raycaster).user;
    Action.Move.take(player);
}