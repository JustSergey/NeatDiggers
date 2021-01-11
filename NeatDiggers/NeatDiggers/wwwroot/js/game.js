import * as THREE from '../lib/three/build/three.module.js';
import { GUI } from '../lib/dat.gui/build/dat.gui.module.js';

let camera, scene, renderer, gui;
let isMyTurn;
let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - 56;
    }
};

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

let conection;

init();

function pointerUp(event) {
    if (!isMyTurn && isDragging) return;
    isDragging = false;
    sendPlayerPosition();
    //send new coords for all players
    dragObject = null;
}

function sendPlayerPosition() {
    let GameActionType = {
        Move: 0,
        Dig: 1,
        Attack: 2,
        UseItem: 3,
        DropItem: 3,
        UseAbility: 5
    };

    let gameAction = {
        targetPosition: {
            x: dragObject.position.x,
            y: dragObject.position.y
        },
        gameActionType: GameActionType.Move
    };

    conection.invoke('DoAction', gameAction).catch(function (err) {
        return console.error(err.toString());
    });
}


function pointerDown(event) {
    if (!isMyTurn) return;
    let intersects = raycaster.intersectObjects(scenePlayer.children);
    if (intersects.length > 0) {
        objIntersect.copy(intersects[0].point);
        plane.setFromNormalAndCoplanarPoint(pNormal, objIntersect);
        shift.subVectors(intersects[0].object.position, intersects[0].point);
        isDragging = true;
        dragObject = intersects[0].object;
    }
}

function pointerMove(event) {
    if (!isMyTurn) return;
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - 28) / window.innerHeight) * 2 + 1;
    raycaster.setFromCamera(mouse, camera);

    if (isDragging) {
        raycaster.ray.intersectPlane(plane, planeIntersect);
        dragObject.position.addVectors(planeIntersect, shift);
        dragObject.position.set(Math.floor(dragObject.position.x), Math.floor(dragObject.position.y), Math.floor(dragObject.position.z))
    }
}

function init() {
    renderInit();
    sceneInit();
    cameraInit();
    document.addEventListener('pointerup', pointerUp, false);
    document.addEventListener('pointerdown', pointerDown, false);
    document.addEventListener('pointermove', pointerMove, false);
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


guiInit();
async function guiInit() {
    gui = new GUI();
    let mainFolder = gui.addFolder("Main");
    let uui = {
        count: 0,
        rollDice: async function () { this.count = await conection.invoke('RollTheDice'); console.log(this.count); },
        endTurn: async function () { await conection.invoke('EndTurn');},
    };
    mainFolder.add(uui, 'rollDice');
    mainFolder.add(uui, 'endTurn');
    mainFolder.add(uui, 'count').listen();

    mainFolder.open();

    let cameraFolder = gui.addFolder("Camera");
    cameraFolder.add(camera.position, "z", 5, 20, 0.01);
    cameraFolder.open();
}

export function animate() {
    requestAnimationFrame(animate);
    update();
    renderer.render(scene, camera);
}

export function setTurn(bool) {
    isMyTurn = bool;
}

function update() {

}

export function UpdateRoom(room, conect) {
    conection = conect;
    DrawFloor(room.gameMap);
    AddSpanPounts(room.gameMap.spawnPoints);
    AddFlag(room.gameMap.flagSpawnPoint);
    SpawnPlayers(room.players, room.userId);
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

        if (players[i].id == userId)
            scenePlayer.add(cube);
        else
            scenePlayers.add(cube);

        cube.position.set(players[i].position.x, players[i].position.y, 1);
    }
}

function DrawFloor(map) {
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

//function move(speed) {
//    var d = mesh.position.x - mesh2.position.x;
//    if (mesh.position.x > mesh2.position.x) {
//        mesh.position.x -= Math.min(speed, d);
//    }
//}