import * as THREE from '../lib/three/build/three.module.js';
import * as core from "./core.js";

import { doAction, invoke, ChangeInventory } from './game.js';
import { checkAvailability, Message, GameActionType, ItemType, Target, WeaponHanded, modelLoader, WeaponType, getKeyByValue, Ability } from './util.js';

const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();

let ui = {
    div: document.createElement("div"),
    contorls: {
        container: document.createElement("div"),
        message: {
            turn: document.createElement("p"),
            error: document.createElement("p"),
            actionCount: document.createElement("p"),
            init: function () {
                this.turn.classList.add("ui");
                this.actionCount.classList.add("ui");
                this.error.classList.add("ui");

                this.error.style.color = "red";

                ui.contorls.container.appendChild(this.turn);
                ui.contorls.container.appendChild(this.actionCount);
                ui.contorls.container.appendChild(this.error);
            },
            update: function (action) {
                if (action != null && action.type == GameActionType.DropItem) return;
                Action.finishAction();
            }
        },
        button: {
            dig: document.createElement("button"),
            move: document.createElement("button"),
            rollDice: document.createElement("button"),
            takeFlag: document.createElement("button"),
            end: document.createElement("button"),
            init: function () {
                this.dig.style.pointerEvents = "all";
                this.move.style.pointerEvents = "all";
                this.rollDice.style.pointerEvents = "all";
                this.takeFlag.style.pointerEvents = "all";
                this.end.style.pointerEvents = "all";

                this.dig.classList.add("ui");
                this.dig.classList.add("btn");
                this.dig.classList.add("btn-success");
                this.move.classList.add("ui");
                this.move.classList.add("btn");
                this.move.classList.add("btn-success");
                this.rollDice.classList.add("ui");
                this.rollDice.classList.add("btn");
                this.rollDice.classList.add("btn-success");
                this.takeFlag.classList.add("ui");
                this.takeFlag.classList.add("btn");
                this.takeFlag.classList.add("btn-success");
                this.end.classList.add("ui");
                this.end.classList.add("btn");
                this.end.classList.add("btn-success");

                this.dig.disabled = true;
                this.move.disabled = true;
                this.takeFlag.disabled = true;

                this.dig.innerText = Message.Button.Dig;
                this.move.innerText = Message.Button.Move;
                this.rollDice.innerText = Message.Button.RollDice;
                this.takeFlag.innerText = Message.Button.TakeFlag;
                this.end.innerText = Message.Button.EndTurn;

                this.dig.onclick = Action.Dig.dig;
                this.move.onclick = function () { Action.Move.init(); } ;
                this.rollDice.onclick = Action.RollDise;
                this.takeFlag.onclick = Action.TakeFlag;
                this.end.onclick = Action.EndTurn;

                ui.contorls.container.appendChild(this.dig);
                ui.contorls.container.appendChild(this.move);
                ui.contorls.container.appendChild(this.rollDice);
                ui.contorls.container.appendChild(this.takeFlag);
                ui.contorls.container.appendChild(this.end);
            },
            update: function () {
                this.rollDice.disabled = Action.count < 1;
            }
        },
        inventory: {
            container: document.createElement("div"),
            leftWeapon: document.createElement("p"),
            leftWeaponTakeOff: document.createElement("button"),
            rightWeapon: document.createElement("p"),
            rightWeaponTakeOff: document.createElement("button"),
            armor: document.createElement("p"),
            armorTakeOff: document.createElement("button"),
            drop: document.createElement("p"),
            items: {
                container: document.createElement("div"),
                title: document.createElement("p"),
                init: function () {
                    this.title.innerText = Message.Inventory.Title;
                    this.container.classList.add("items");
                    ui.contorls.inventory.container.appendChild(this.title);
                    ui.contorls.inventory.container.appendChild(this.container);
                },
                update: function (items) {
                    this.clear();

                    for (var i = 0; i < items.length; i++) {
                        let item = items[i];
                        let itemUse = document.createElement("button");
                        let itemDrop = document.createElement("button");
                        let itemDescription = document.createElement("p");
                        itemDescription.style.marginBottom = "0";
                        itemDescription.style.marginTop = "1.25rem";
                        itemUse.classList.add("ui");
                        itemUse.classList.add("btn");
                        itemUse.classList.add("btn-success");
                        itemDrop.classList.add("ui");
                        itemDrop.classList.add("btn");
                        itemDrop.classList.add("btn-success");
                        let itemRight = null;
                        itemUse.style.pointerEvents = "all";
                        switch (item.type) {
                            case ItemType.Active:
                                itemUse.classList.add("itemButton");
                                itemUse.disabled = Action.count < 1;
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
                                let canUse = true;
                                if (core.sPlayer.info.character.weaponType != WeaponType.None)
                                    canUse = core.sPlayer.info.character.weaponType == item.weaponType;

                                switch (item.weaponHanded) {
                                    case WeaponHanded.One:
                                        itemRight = document.createElement("button");
                                        itemRight.style.pointerEvents = "all";
                                        itemRight.classList.add("itemButton");
                                        itemRight.classList.add("btn");
                                        itemRight.classList.add("btn-success");
                                        itemUse.innerText = Message.Button.Equip.Left;
                                        itemRight.innerText = Message.Button.Equip.Right;
                                        itemUse.onclick = function () { ItemsActions.equipLeft(item); };
                                        itemRight.onclick = function () { ItemsActions.equipRight(item); };
                                        itemUse.disabled = !canUse;
                                        itemRight.disabled = !canUse;
                                        itemRight.classList.add("ui");
                                        break;
                                    case WeaponHanded.Two:
                                        itemUse.disabled = !canUse;
                                        itemUse.innerText = Message.Button.Equip.Two;
                                        itemUse.onclick = function () { ItemsActions.equipTwo(item); };
                                        break;
                                    default:
                                }
                                break;
                        }
                        itemDrop.style.pointerEvents = "all";
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
                ui.contorls.container.appendChild(this.container);

                this.leftWeaponTakeOff.style.display = "none";
                this.leftWeaponTakeOff.style.pointerEvents = "all";
                this.leftWeaponTakeOff.classList.add("itemButton-nohiden");
                this.leftWeaponTakeOff.classList.add("btn");
                this.leftWeaponTakeOff.classList.add("btn-success");
                this.rightWeaponTakeOff.style.display = "none";
                this.rightWeaponTakeOff.style.pointerEvents = "all";
                this.rightWeaponTakeOff.classList.add("itemButton-nohiden");
                this.rightWeaponTakeOff.classList.add("btn");
                this.rightWeaponTakeOff.classList.add("btn-success");
                this.armorTakeOff.style.display = "none";
                this.armorTakeOff.style.pointerEvents = "all";
                this.armorTakeOff.classList.add("itemButton-nohiden");
                this.armorTakeOff.classList.add("btn");
                this.armorTakeOff.classList.add("btn-success");

                this.container.appendChild(this.leftWeapon);
                this.container.appendChild(this.leftWeaponTakeOff);
                this.container.appendChild(this.rightWeapon);
                this.container.appendChild(this.rightWeaponTakeOff);
                this.container.appendChild(this.armor);
                this.container.appendChild(this.armorTakeOff);
                this.container.appendChild(this.drop);
                this.items.init();
            },
            update: function (inventory) {
                this.leftWeapon.innerText = Message.Inventory.LeftWeapon;
                this.armor.innerText = Message.Inventory.Armor;
                this.rightWeapon.innerText = Message.Inventory.RightWeapon;


                if (inventory.rightWeapon.title != null) {
                    this.rightWeapon.innerText += inventory.rightWeapon.title + " (" + inventory.rightWeapon.description + ")";
                    this.rightWeaponTakeOff.style.display = "block";
                    this.rightWeaponTakeOff.innerText = Message.TakeOff;
                    this.rightWeaponTakeOff.onclick = function () {
                        ItemsActions.takeOff(inventory.rightWeapon);
                        ui.contorls.inventory.rightWeaponTakeOff.style.display = "none";
                    };
                }
                else if (inventory.leftWeapon.weaponHanded == WeaponHanded.Two) {
                    this.leftWeapon.innerText = Message.Inventory.Two;
                    this.leftWeaponTakeOff.style.display = "block";
                    this.rightWeapon.innerText = "";
                    ui.contorls.inventory.rightWeaponTakeOff.style.display = "none";
                }
                if (inventory.leftWeapon.title != null) {
                    this.leftWeapon.innerText += inventory.leftWeapon.title + " (" + inventory.leftWeapon.description + ")";
                    this.leftWeaponTakeOff.style.display = "block";
                    this.leftWeaponTakeOff.innerText = Message.TakeOff;
                    this.leftWeaponTakeOff.onclick = function () {
                        ItemsActions.takeOff(inventory.leftWeapon);
                        this.leftWeaponTakeOff.style.display = "none";
                    };
                }


                if (inventory.armor.title != null) {
                    this.armor.innerText += inventory.armor.title + " (" + inventory.armor.description + ")";
                    this.armorTakeOff.style.display = "block";
                    this.armorTakeOff.innerText = Message.TakeOff;
                    this.armorTakeOff.onclick = function () {
                        ItemsActions.takeOff(inventory.armor);
                        ui.contorls.inventory.armorTakeOff.style.display = "none";
                    };
                }

                this.drop.innerText = Message.Inventory.Drop + inventory.drop;
                this.items.update(inventory.items);
            }
        },
        init: function () {
            this.container.classList.add("contorls");
            this.container.classList.add("col-3");
            this.container.style.textAlign = "left";
            ui.div.appendChild(this.container);

            this.message.init();
            this.button.init();
            this.inventory.init();
        },
        update: function (action, inventory) {
            this.message.update(action);
            this.button.update();
            this.inventory.update(inventory);
        }
    },
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
                this.container.classList.add("abilities");
                ui.player.container.appendChild(this.title);
                ui.player.container.appendChild(this.container);
            },
            update: function (abilities) {
                this.clear();
                for (var i = 0; i < abilities.length; i++) {
                    let description = document.createElement("p");
                    if (abilities[i].isActive)
                        description.style.color = "#ffffff";
                    else
                        description.style.color = "#b0b0b0";

                    description.innerText = getKeyByValue(Ability.Name, abilities[i].name) + " (" + abilities[i].description + ")";
                    description.style.marginBottom = "0";
                    description.style.marginTop = "1.25rem";
                    this.container.appendChild(description);

                    if (abilities[i].type == Ability.Type.Active) {
                        let ability = abilities[i];
                        let abilityUse = document.createElement("button");
                        abilityUse.disabled = !(Action.count > 0 && ability.isActive);
                        abilityUse.classList.add("ui");
                        abilityUse.classList.add("btn");
                        abilityUse.classList.add("btn-success");

                        abilityUse.classList.add("itemButton");
                        abilityUse.style.pointerEvents = "all";
                        switch (ability.target) {
                            case Target.None:
                                abilityUse.innerText = Message.Button.Use.None;
                                abilityUse.onclick = function () { AbilitiesActions.use(ability); };
                                break;
                            case Target.Player:
                                abilityUse.innerText = Message.Button.Use.Player;
                                abilityUse.onclick = function () { AbilitiesActions.onPlayer.init(ability); };
                                break;
                            case Target.Position:
                                abilityUse.innerText = Message.Button.Use.Position;
                                abilityUse.onclick = function () { AbilitiesActions.onPosition.init(ability); };
                                break;
                            default:
                        }
                        this.container.appendChild(abilityUse);
                    }
                }
            },
            clear: function () { ui.clear(this.container); }
        },
        effects: {
            container: document.createElement("div"),
            title: document.createElement("p"),
            init: function () {
                this.title.innerText = Message.Effects;
                this.container.classList.add("effects");
                ui.player.container.appendChild(this.title);
                ui.player.container.appendChild(this.container);
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
            this.container.classList.add("col-3");
            this.container.style.pointerEvents = "none";
            this.container.style.color = "white";
            this.container.style.textAlign = "right";
            this.container.appendChild(this.name);
            this.container.appendChild(this.health);
            this.container.appendChild(this.level);
            this.container.appendChild(this.score);
            this.container.appendChild(this.weaponType);

            this.abilities.init();
            this.effects.init();
            ui.div.appendChild(this.container);
        },
        update: function (player) {
            this.name.innerText = player.name + " (" + player.character.title + ")";
            this.health.innerText = Message.Health + player.health + "/" + player.character.maxHealth;
            this.level.innerText = Message.Level + player.level;
            this.score.innerText = Message.Score + player.score;
            this.weaponType.innerText = Message.WeaponType + getKeyByValue(WeaponType, player.character.weaponType);

            this.abilities.update(player.character.abilities);
            this.effects.update(player.effects);
        }
    },
    log: {
        container: document.createElement("div"),
        log: new Array(),
        init: function () {
            this.container.classList.add("log");
            this.container.classList.add("col-6");
            this.container.style.textAlign = "center";
            ui.div.appendChild(this.container);
        },
        update(action) {
            if (action != null && action.type != GameActionType.Move && action.type != GameActionType.Dig && action.type != GameActionType.DropItem) {
                this.log.push(action);
                this.log = this.log.slice(Math.max(this.log.length - 3, 0));

                this.clear();
                for (var i = 0; i < this.log.length; i++) {
                    let message = document.createElement("p");
                    let targetPlayer;
                    let targetPosition;
                    let currentPlayer;
                    switch (this.log[i].type) {
                        case GameActionType.Attack:
                            targetPlayer = (core.getPlayer(this.log[i].targetPlayerId)).info;
                            currentPlayer = this.log[i].currentPlayer;
                            message.innerText = currentPlayer.name + " (" + currentPlayer.character.title + ") " + " атаковал " +
                                targetPlayer.name + " (" + targetPlayer.character.title + ").";
                            break;
                        case GameActionType.TakeTheFlag:
                            currentPlayer = this.log[i].currentPlayer;
                            message.innerText = currentPlayer.name + " (" + currentPlayer.character.title + ") " + " взял флаг.";
                            break;
                        case GameActionType.UseAbility:
                            break;
                        case GameActionType.UseItem:
                            currentPlayer = this.log[i].currentPlayer;
                            let item = this.log[i].item;
                            message.innerText = currentPlayer.name + " (" + currentPlayer.character.title + ") " + " использовал предмет " + item.title;
                            switch (item.target) {
                                case Target.Player:
                                    targetPlayer = (core.getPlayer(this.log[i].targetPlayerId)).info;
                                    message.innerText += " на " + targetPlayer.name + " (" + targetPlayer.character.title + ").";
                                    break;
                                case Target.Position:
                                    targetPosition = this.log[i].targetPosition;
                                    message.innerText += " на позицию {x:" + targetPosition.x + ",y:" + targetPosition.y + "}.";
                                    break;
                            }
                            break;
                    }
                    this.container.appendChild(message);
                }
            }
        },
        clear: function () { ui.clear(this.container); }
    },
    init: function () {
        this.div.style.position = 'absolute';
        this.div.style.pointerEvents = "none";
        this.div.style.width = window.innerWidth + 'px';
        this.div.style.height = window.innerWidth + 'px';
        this.div.style.margin = 'auto';
        this.div.style.color = "white";
        this.div.style.top = $('header').outerHeight() + 'px';
        this.div.classList.add("gui");
        this.div.classList.add("row");
        document.body.appendChild(this.div);

        this.hint.container.id = "hint";
        this.div.appendChild(this.hint.container);

        this.contorls.init();
        this.log.init();
        this.player.init();
        window.addEventListener('resize', this.resize, false);
        $(".ui").hide();
    },
    update: function (player, action, isMyTurn) {
        this.contorls.update(action, player.inventory);
        this.log.update(action);
        this.player.update(player);

        if (isMyTurn)
            $(".ui").show();
        else {
            $(".ui").hide();
            $(".itemButton-nohiden").hide();
        }
    },
    resize: function () {
        ui.div.style.width = window.innerWidth + 'px';
        ui.div.style.height = window.innerWidth + 'px';
    },
    clear: function (container) {
        while (container.firstChild)
            container.removeChild(container.firstChild);
    }
}
let target;

export function ShowTakeFlagButton(isActive) {
    ui.contorls.button.takeFlag.disabled = !isActive;
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
        ui.contorls.message.turn.innerText = Message.ActionRemains + Action.count;
        Action.diceValue = -1;
        ui.contorls.message.actionCount.innerText = Message.NeedRollDice;
        ui.contorls.button.dig.disabled = !isPlayerCanDig();
        ui.contorls.button.move.disabled = true;
        if (this.count < 1) {
            ui.contorls.button.dig.disabled = true;
            ui.contorls.button.rollDice.disabled = true;
            ui.contorls.button.takeFlag.disabled = true;
            ui.contorls.message.actionCount.innerText = "";
            $(".itemButton").prop("disabled", true);
        }
    },
    Move: {
        Can: true,
        listen: false,
        init: function () {
            this.listen = true;
            target.visible = true;
        },
        showHint: function () {
            if (!this.listen || Action.count < 1 || !Action.Move.Can) {
                ui.hint.hide();
                ui.hint.clear();
                return;
            }
            target.visible = false;
            let btn = document.createElement('button');
            btn.style.pointerEvents = "all";
            btn.innerText = "Move";
            btn.onmousedown = async function () {
                let action = {
                    Type: GameActionType.Move,
                    TargetPosition: target.position,
                };
                let success = await doAction(action);

                if (success) {
                    Action.count--;
                    Action.finishAction();
                    Action.Move.Can = false;
                    ui.contorls.button.move.disabled = true;
                }
                ui.hint.hide();
                ui.hint.clear();
                target.position.set();
            }
            ui.hint.container.appendChild(btn);
            ui.hint.show();
            this.listen = false;
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
                ui.contorls.button.dig.disabled = true;
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
                    ui.hint.clear();
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
        ui.contorls.message.actionCount.innerText = Action.diceValue;
        ui.contorls.button.dig.disabled = !isPlayerCanDig();
        ui.contorls.button.move.disabled = !Action.Move.Can;
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
            ui.contorls.button.dig.disabled = true;
        }
    },
    TakeFlag: async function () {
        let action = {
            Type: GameActionType.TakeTheFlag
        };
        let success = await doAction(action);
        if (success) {
            ui.contorls.button.takeFlag.disabled = true;
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

let AbilitiesActions = {
    use: async function (ability) {
        let action = {
            Type: GameActionType.UseAbility,
            Ability: ability
        };
        let success = await doAction(action);
        if (success) {
            Action.count--;
            Action.finishAction();
        }
    },
    onPlayer: {
        listen: false,
        ability: null,
        init: function (ability) {
            this.ability = ability;
            this.listen = true;
            target.visible = true;
        },
        showHint: function (players) {
            if (!this.listen || Action.count < 1) {
                ui.hint.hide();
                ui.hint.clear();
                return;
            }
            target.visible = false;
            for (var i = 0; i < players.length; i++) {
                let btn = document.createElement('button');
                btn.style.pointerEvents = "all";

                let playerId = players[i].info.id;
                let playerName = players[i].info.name;
                btn.innerText = "Использовать на: " + playerName;
                btn.onmousedown = async function () {
                    let action = {
                        Type: GameActionType.UseAbility,
                        TargetPlayerId: playerId,
                        Ability: AbilitiesActions.onPlayer.ability
                    }
                    let success = await doAction(action);

                    if (success) {
                        Action.count--;
                        Action.finishAction();
                    }

                    ui.hint.hide();
                    ui.hint.clear();
                    target.position.set();
                    this.ability = null;
                }
                ui.hint.container.appendChild(btn);
                ui.hint.show();
                this.listen = false;
            }
        }
    },
    onPosition: {
        listen: false,
        ability: null,
        init: function (ability) {
            this.ability = ability;
            this.listen = true;
            target.visible = true;
        },
        use: async function () {
            if (!this.listen || Action.count < 1) return;
            let action = {
                Type: GameActionType.UseAbility,
                TargetPosition: target.position,
                Ability: this.ability
            };
            let success = await doAction(action);
            target.visible = false;
            if (success) {
                Action.count--;
                Action.finishAction();
            }

            this.listen = false;
            this.ability = null;
            target.position.set();
        }
    }
}

let ItemsActions = {
    takeOff: function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.leftWeapon != null && inventory.leftWeapon.name == item.name) {
            inventory.items.push(inventory.leftWeapon);
            inventory.leftWeapon = { name: 0 };
        }
        if (inventory.rightWeapon != null && inventory.rightWeapon.name == item.name) {
            inventory.items.push(inventory.rightWeapon);
            inventory.rightWeapon = { name: 0 };
        }
        if (inventory.armor != null && inventory.armor.name == item.name) {
            inventory.items.push(inventory.armor);
            inventory.armor = { name: 0 };
        }
        ChangeInventory(inventory);
    },
    drop: function (item) {
        let action = {
            Type: GameActionType.DropItem,
            Item: item
        };
        doAction(action);
    },
    equipArmor: async function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.armor != null && inventory.armor.title != null)
            inventory.items.push(inventory.armor);
        inventory.armor = item;
        arrayRemove(inventory.items, item);

        ChangeInventory(inventory);
    },
    equipLeft: async function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.leftWeapon != null && inventory.leftWeapon.title != null)
            inventory.items.push(inventory.leftWeapon);
        inventory.leftWeapon = item;
        arrayRemove(inventory.items, item);

        ChangeInventory(inventory);
    },
    equipRight: function (item) {
        let inventory = core.sPlayer.info.inventory;
        if (inventory.leftWeapon != null && inventory.leftWeapon.weaponHanded == WeaponHanded.Two) {
            inventory.items.push(inventory.leftWeapon);
            inventory.leftWeapon = null;
        }

        if (inventory.rightWeapon != null && inventory.rightWeapon.title != null)
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
            if (!this.listen || Action.count < 1) {
                ui.hint.hide();
                ui.hint.clear();
                return;
            }
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
                    ui.hint.clear();
                    this.item = null;
                    target.position.set();
                }
                ui.hint.container.appendChild(btn);
                ui.hint.show();
                this.listen = false;
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
        },
        use: async function () {
            if (!this.listen || Action.count < 1) return;
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

    ui.hint.setPosition(event.clientX, event.clientY);
    let players = getPlayers(raycaster).other;
    if (players.length > 0) {
        ui.hint.clear();
        
        Action.Attack.showHint(players);
        ItemsActions.onPlayer.showHint(players);
        AbilitiesActions.onPlayer.showHint(players);
    }

    if (!isMyTurn || Action.count < 1 || target.position.x == undefined || target.position.y == undefined) return;
    Action.Move.showHint();
    ItemsActions.onPosition.use();
    AbilitiesActions.onPosition.use();
}

function pointerMove(event) {
    if (!isMyTurn || Action.count < 1) return;

    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - ($('header').outerHeight() / 2)) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    if (target.visible) {
        let intersectsObjects = raycaster.intersectObjects(core.scene.children);
        if (intersectsObjects.length > 0)
            target.position.set(intersectsObjects[0].object.position.x, intersectsObjects[0].object.position.y, 0);
        else
            target.position.set();
    }
}