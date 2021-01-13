import * as THREE from '../lib/three/build/three.module.js';
import * as actions from "./actions.js";
import * as util from "./util.js"

let camera, scene, renderer;
export let sFlag, sPlayer, sPlayers = new THREE.Group();

let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - $('header').outerHeight();
        renderer = new THREE.WebGLRenderer();
        renderer.setSize(screen.width, screen.height);
        window.addEventListener('resize', this.onResize, false);
    },
    'onResize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - $('header').outerHeight();

        camera.aspect = this.width / this.height;
        camera.updateProjectionMatrix();
        renderer.setSize(this.width, this.height);
    }
};

export function init(map) {
    renderInit();
    cameraInit();
    sceneInit();
    drawMap(map);
    animate();
    actions.setCamera(camera);
}

function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
}

export function updatePlayers(players, userId) {
    placePlayers(players, userId);
    removeReplasedPlayers(players);
}

function removeReplasedPlayers(players) {
    for (var i = 0; i < sPlayers.children.length; i++) {
        let exist;
        for (var j = 0; j < players.length; j++)
            exist = exist || (sPlayers.children[i].info.id == players[j].id);
        if (!exist)
            sPlayers.remove(sPlayers.children[i]);
    }
}

function placePlayers(players, userId) {
    const boxGeometry = new THREE.BoxGeometry();

    for (var i = 0; i < players.length; i++) {
        if (!isExistPlayer(players[i].id)) {
            let material = new THREE.MeshPhongMaterial({ color: 0x6cc924 });
            let cube = new THREE.Mesh(boxGeometry, material);
            cube.position.set(players[i].position.x, players[i].position.y, 1);
            cube.info = players[i];
            sPlayers.add(cube);
            if (players[i].id == userId) {
                sPlayer = cube;
            }
        }
    }
}

//export function moveFlag(pos) {
//    util.move(sFlag, pos);
//}

function drawMap(map) {
    drawFloor(map);
    drawSpawnPoints(map.spawnPoints);
    drawFlag(map.flagSpawnPoint);
}

function drawFlag(pos) {
    const boxGeometry = new THREE.BoxGeometry();
    let material = new THREE.MeshPhongMaterial();
    sFlag = new THREE.Mesh(boxGeometry, material);
    scene.add(sFlag);
    sFlag.position.set(pos.x, pos.y, 1);
    sFlag.scale.set(0.2, 0.2, 1);
}

function drawFloor(map) {
    const Cell = {
        None: 0,
        Empty: 1,
        Wall: 2,
        Digging: 3
    };

    const boxGeometry = new THREE.BoxGeometry();
    let materialEmpty = new THREE.MeshPhongMaterial({ color: 0x464646 });
    let materialWall = new THREE.MeshPhongMaterial({ color: 0x2ce900 });
    let materialDigging = new THREE.MeshPhongMaterial({ color: 0xeb0034 });

    for (var x = 0; x < map.width; x++) {
        for (var y = 0; y < map.height; y++) {
            if (map.map[x * map.width + y] != Cell.None) {
                let cube;
                switch (map.map[x * map.width + y]) {
                    case Cell.Empty: cube = new THREE.Mesh(boxGeometry, materialEmpty); break;
                    case Cell.Wall: cube = new THREE.Mesh(boxGeometry, materialWall); break;
                    case Cell.Digging: cube = new THREE.Mesh(boxGeometry, materialDigging); break;
                }
                scene.add(cube);
                cube.position.set(x, y);
            }
        }
    }
}

function drawSpawnPoints(spawnPoints) {
    const boxGeometry = new THREE.BoxGeometry();
    let material = new THREE.MeshPhongMaterial();

    for (var i = 0; i < spawnPoints.length; i++) {
        const cube = new THREE.Mesh(boxGeometry, material);
        scene.add(cube);
        cube.position.set(spawnPoints[i].x, spawnPoints[i].y, 0.01);
    }
}

function renderInit() {
    screen.resize();
    document.body.appendChild(renderer.domElement).id = 'game';
    $("#game").hide();
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

    scene.add(sPlayers);
}

function isExistPlayer(userId) {
    for (var i = 0; i < sPlayers.children.length; i++)
        if (sPlayers.children[i].info.id == userId)
            return true;
    return false;
}