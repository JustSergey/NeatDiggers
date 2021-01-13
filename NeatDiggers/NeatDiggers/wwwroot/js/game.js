import * as THREE from '../lib/three/build/three.module.js';
import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';

let camera, scene, renderer;
let loader;
let isMyTurn;
let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - 56;
    }
};

let pandora;

const scenePlayers = new THREE.Group();
const scenePlayer = new THREE.Group();
const BoxGeometry = new THREE.BoxGeometry();
const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();

let plane = new THREE.Plane();
let pNormal = new THREE.Vector3(0, 0, 1);
let planeIntersect = new THREE.Vector3();
let objIntersect = new THREE.Vector3();
let shift = new THREE.Vector3();
let isDragging = false;
let dragObject;
let oldPosition = new THREE.Vector3();

let connection;

let log, inventory, turn, count;

let btnRollDice, hint, playerHealth, btnDig, canMove = true, canAttack = true;

let GameActionType = {
    Move: 0,
    Dig: 1,
    Attack: 2,
    UseItem: 3,
    DropItem: 4,
    UseAbility: 5
};

init();

async function pointerUp(event) {
    if (!isMyTurn) return;

    let intersectsOtherPlayers = raycaster.intersectObjects(scenePlayers.children);
    if (intersectsOtherPlayers.length > 0 && canAttack) {

        let radius = await connection.invoke("GetAttackRadius").catch(function (err) {
            return console.error(err.toString());
        });

        while (hint.firstChild) {
            hint.removeChild(hint.firstChild);
        }
        for (var i = 0; i < intersectsOtherPlayers.length; i++) {
            if (!CheckAvailability(pandora.position, intersectsOtherPlayers[i].object.position, radius))
                continue;
            let btn = document.createElement('button');
            let playerId = intersectsOtherPlayers[i].object.player.id;
            btn.innerText = playerId;

            btn.onclick = function () {
                let gameAction = {
                    Type: GameActionType.Attack,
                    TargetPlayerId: playerId
                };
                doAction(gameAction);
                hint.style.display = 'none';
            }

            hint.appendChild(btn);
            hint.style.display = 'block';
        }
    }

    if (!isDragging) return;
    isDragging = false;
    sendPlayerPosition();
}

async function sendPlayerPosition() {
    let gameAction = {
        targetPosition: {
            x: dragObject.position.x,
            y: dragObject.position.y
        },
        Type: GameActionType.Move
    };

    let can = await doAction(gameAction);

    if (!can) {
        pandora.position.set(oldPosition.x, oldPosition.y, 0);
    }
}

function pointerDown(event) {
    if (!isMyTurn || actionsCount < 1) return;
    let intersectsPlayer = raycaster.intersectObjects(scenePlayer.children);
    if (intersectsPlayer.length > 0 && canMove) {
        objIntersect.copy(intersectsPlayer[0].point);
        plane.setFromNormalAndCoplanarPoint(pNormal, objIntersect);
        shift.subVectors(intersectsPlayer[0].object.position, intersectsPlayer[0].point);
        isDragging = true;
        dragObject = intersectsPlayer[0].object;
        oldPosition.x = intersectsPlayer[0].object.position.x;
        oldPosition.y = intersectsPlayer[0].object.position.y;
    }

}

function CheckAvailability(startPoint, targetPoint, radius) {
    let x = targetPoint.x - startPoint.x;
    let y = targetPoint.y - startPoint.y;
    return radius >= Math.round(Math.sqrt(x * x + y * y));
}

function pointerMove(event) {
    if (!isMyTurn) return;
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - 28) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    if (isDragging) {
        raycaster.ray.intersectPlane(plane, planeIntersect);
        dragObject.position.addVectors(planeIntersect, shift);
        dragObject.position.set(Math.round(dragObject.position.x), Math.round(dragObject.position.y), 0)
    }
}

function init() {
    renderInit();
    sceneInit();
    cameraInit();
    loaderInit();
    document.addEventListener('pointerup', pointerUp, false);
    document.addEventListener('pointerdown', pointerDown, false);
    document.addEventListener('pointermove', pointerMove, false);
}

function loaderInit() {
    loader = new GLTFLoader();

    loader.load('../../StaticFiles/models/pandora.glb', function (gltf) {
        pandora = gltf.scene.children[0];
    }, undefined, function (error) {
        console.error(error);
    });
}

function renderInit() {
    screen.resize();
    renderer = new THREE.WebGLRenderer();
    renderer.setSize(screen.width, screen.height);
    document.body.appendChild(renderer.domElement).id = 'game';
    document.body.style.marginBottom = 0;
}

function cameraInit() {
    camera = new THREE.PerspectiveCamera(75, screen.width / screen.height, 0.1, 50);
    camera.position.set(6, 6, 10);
}

function sceneInit() {
    scene = new THREE.Scene();

    let hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444);
    hemiLight.position.set(0, 20, 0);
    scene.add(hemiLight);

    scene.add(scenePlayers);
    scene.add(scenePlayer);
}

let isInitGui;
export async function guiInit() {
    if (isInitGui) return;
    isInitGui = true;
    let div = document.createElement("div");
    div.style.position = 'absolute';
    div.style.border = '1px solid black';
    div.style.width = screen.width + 'px';
    div.style.height = screen.height + 'px';
    div.style.margin = 'auto';
    div.style.top = 56 + 'px';
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
    count.onselectstart = false;
    count.onmousedown = false;
    div.appendChild(count);

    let uui = {
        rollDice: async function () {
            count.innerText = await connection.invoke('RollTheDice').catch(function (err) {
                return console.error(err.toString());
            });
        },
        endTurn: async function () {
            let success = await connection.invoke('EndTurn').catch(function (err) {
                return console.error(err.toString());
            });
            if (success) {
                actionsCount = 2;
                canMove = true;
                canAttack = true;
                btnDig.disabled = false;
                btnRollDice.disabled = false;
                count.innerText = "You need to roll the dice";
            }
        },
        dig: function () {
            let gameAction = {
                Type: GameActionType.Dig,
                targetPosition: {
                    x: pandora.position.x,
                    y: pandora.position.y
                }
            };
            doAction(gameAction);
        }
    };

    btnRollDice = document.createElement("button");
    btnRollDice.innerText = 'rollDice';
    btnRollDice.onclick = uui.rollDice;
    btnRollDice.onselectstart = false;
    btnRollDice.onmousedown = false;
    div.appendChild(btnRollDice);

    btnDig = document.createElement("button");
    btnDig.innerText = 'Dig';
    btnDig.onclick = uui.dig;
    btnDig.onselectstart = false;
    btnDig.onmousedown = false;
    div.appendChild(btnDig);

    let btnEndTurn = document.createElement("button");
    btnEndTurn.innerText = 'EndTurn';
    btnEndTurn.onclick = uui.endTurn;
    btnEndTurn.onselectstart = false;
    btnEndTurn.onmousedown = false;
    div.appendChild(btnEndTurn);

    inventory = document.createElement("div");
    inventory.style.color = "white";
    inventory.onselectstart = false;
    inventory.onmousedown = false;
    div.appendChild(inventory);

    log = document.createElement("p");
    log.innerText = 'log';
    log.style.color = "white";
    log.onselectstart = false;
    log.onmousedown = false;
    //div.appendChild(log);
}

let actionsCount = 2;
async function doAction(gameAction) {
    let success = await connection.invoke('DoAction', gameAction).catch(function (err) {
        return console.error(err.toString());
    });
    if (success) {
        switch (gameAction.Type) {
            case GameActionType.Move:
                canMove = false;
                break;
            case GameActionType.Dig:
                btnDig.disabled = true;
                break;
            case GameActionType.Attack:
                canAttack = false;
                break;
            //case GameActionType.UseAbility: break;
            //case GameActionType.UseItem: break;
        }
        if (gameAction.Type != GameActionType.DropItem) {
            actionsCount--;
            count.innerText = "You need to roll the dice";
            turn.innerText = "Actions remains: " + actionsCount;
        }
        if (actionsCount < 1)
            btnRollDice.disabled = true;
    }
    return success;
}

export function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

export function setTurn(bool) {
    isMyTurn = bool;
}

export function UpdateRoom(room, conect) {
    connection = conect;
    SpawnPlayers(room.players, room.userId);
    log.innerHTML = JSON.stringify(room);

    if (room.players[room.playerTurn].id == room.userId) {
        playerHealth.innerText = "Health: " + room.players[room.playerTurn].health + "/" + room.players[room.playerTurn].character.maxHealth;
        UpdateInventory(room.players[room.playerTurn].inventory);
        turn.innerText = "Your move!";
        turn.style.visibility = 'visible';
        count.style.visibility = 'visible';
    }
    else {
        turn.style.visibility = 'hidden';
        count.style.visibility = 'hidden';
    }
}

function UpdateInventory(inv) {
    ClearInventory();
    AddOutfit(inv);
    AddItems(inv.items);
}

function AddItems(items) {
    for (var i = 0; i < items.length; i++) {
        let item = items[i];
        let itemTitle = document.createElement("button");
        itemTitle.innerText = item.title;
        
        let itemDescription = document.createElement("p");
        itemDescription.innerText = item.description;
        let itemDrop = document.createElement("button");
        itemDrop.innerText = "Drop";
        itemDrop.onclick = function () {
            if (!isMyTurn) return;
            let gameAction = {
                Type: GameActionType.DropItem,
                Item: item,
                targetPosition: {
                    x: pandora.position.x,
                    y: pandora.position.y
                }
            };
            doAction(gameAction);
        }

        inventory.appendChild(itemTitle);
        inventory.appendChild(itemDrop);
        inventory.appendChild(itemDescription);
    }
}

function AddOutfit(inv) {

}

function ClearInventory() {
    while (inventory.firstChild) {
        inventory.removeChild(inventory.firstChild);
    }
}

function SpawnPlayers(players, userId) {
    for (var i = 0; i < scenePlayers.children.length; i++) {
        scenePlayers.remove(scenePlayers.children[0]);
    }

    for (var i = 0; i < scenePlayer.children.length; i++) {
        scenePlayer.remove(scenePlayer.children[0]);
    }

    for (var i = 0; i < players.length; i++) {
        let material = new THREE.MeshPhongMaterial({ color: 0x6cc924 });
        let cube = new THREE.Mesh(BoxGeometry, material);

        if (players[i].id == userId) {
            scenePlayer.add(pandora);
            pandora.position.set(players[i].position.x, players[i].position.y, 0);
            pandora.player = players[i];
        }
        else {
            scenePlayers.add(cube);
            cube.position.set(players[i].position.x, players[i].position.y, 1);
            cube.player = players[i];
        }
    }
}

export function DrawMap(map) {
    let Cell = {
        None: 0,
        Empty: 1,
        Wall: 2,
        Digging: 3
    };
    for (var x = 0; x < map.width; x++) {
        for (var y = 0; y < map.height; y++) {
            if (map.map[x * map.width + y] != Cell.None) {
                let material = new THREE.MeshPhongMaterial({ color: 0xffffff });
                if (map.map[x * map.width + y] == Cell.Empty) {
                    material = new THREE.MeshPhongMaterial({ color: 0x464646 });
                } else if (map.map[x * map.width + y] == Cell.Wall) {
                    material = new THREE.MeshPhongMaterial({ color: 0x2ce900 });
                } else if (map.map[x * map.width + y] == Cell.Digging) {
                    material = new THREE.MeshPhongMaterial({ color: 0xeb0034 });
                }
                const cube = new THREE.Mesh(BoxGeometry, material);
                scene.add(cube);
                cube.position.set(x, y, 0);
            }
        }
    }

    AddSpanPounts(map.spawnPoints);
    AddFlag(map.flagSpawnPoint);

    const gridHelper = new THREE.GridHelper(map.width, map.height, new THREE.Color(0x000000), new THREE.Color(0x000000));
    gridHelper.rotateX(Math.PI / 2);
    gridHelper.position.set((map.width - 1) / 2, (map.height - 1) / 2, 0.51);
    scene.add(gridHelper);
}

function AddSpanPounts(spawnPoints) {
    for (var i = 0; i < spawnPoints.length; i++) {
        let material = new THREE.MeshPhongMaterial({ color: 0xffffff });
        const cube = new THREE.Mesh(BoxGeometry, material);
        scene.add(cube);
        cube.position.set(spawnPoints[i].x, spawnPoints[i].y, 0.01);
    }
}

function AddFlag(flagSpawnPoint) {
    let material = new THREE.MeshPhongMaterial({ color: 0x00089d, depthWrite: false });
    let cube = new THREE.Mesh(BoxGeometry, material);
    scene.add(cube);
    cube.position.set(flagSpawnPoint.x, flagSpawnPoint.y, 1);
    cube.scale.set(0.2, 0.2, 1);
}