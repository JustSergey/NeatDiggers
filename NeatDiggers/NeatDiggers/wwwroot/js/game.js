import * as THREE from '../lib/three/build/three.module.js';

let camera, scene, renderer;
let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - 56;
    }
};

const scenePlayers = new THREE.Group();
const BoxGeometry = new THREE.BoxGeometry();
const raycaster = new THREE.Raycaster();
const mouse = new THREE.Vector2();

init();

function init() {
    renderInit();
    cameraInit();
    sceneInit();
    //window.addEventListener('mousemove', onMouseMove, false);
    window.addEventListener('mousedown', onMouseMove, false);
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
    camera.rotation.z = Math.PI / 2;
    camera.position.y = 6;
    camera.position.x = 6;
    camera.position.z = 10;
}

function sceneInit() {
    scene = new THREE.Scene();

    let hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444);
    hemiLight.position.set(0, 20, 0);
    scene.add(hemiLight);

    scene.add(scenePlayers);
}

export function animate() {
    requestAnimationFrame(animate);
    update();
    renderer.render(scene, camera);
}

function update() {

}

function onMouseMove(event) {
    mouse.x = (event.clientX / window.innerWidth) * 2 - 1;
    mouse.y = - ((event.clientY - 28) / window.innerHeight) * 2 + 1;
    setRedColorRay();
}

function setRedColorRay() {
    raycaster.setFromCamera(mouse, camera);
    const intersects = raycaster.intersectObjects(scenePlayers.children);
    for (let i = 0; i < intersects.length; i++) {
        intersects[i].object.material.color.set(0xff0000);
    }
}

export function UpdateRoom(room) {
    DrawFloor(room.gameMap);
    AddSpanPounts(room.gameMap.spawnPoints);
    AddFlag(room.gameMap.flagSpawnPoint);

    SpawnPlayers(room.players);
}

function SpawnPlayers(players) {
    for (var i = 0; i < scenePlayers.children.length; i++) {
        scenePlayers.remove(scenePlayers.children[0]);
    }

    for (var i = 0; i < players.length; i++) {
        let material = new THREE.MeshPhongMaterial({ color: 0x6cc924, depthWrite: false });
        let cube = new THREE.Mesh(BoxGeometry, material);
        scenePlayers.add(cube);
        cube.position.x = players[i].position.x;
        cube.position.y = players[i].position.y;
        cube.position.z = 1;
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
                cube.position.x = x;
                cube.position.y = y;
            }
        }
    }
}

function AddSpanPounts(spawnPoints) {
    for (var i = 0; i < spawnPoints.length; i++) {
        let material = new THREE.MeshPhongMaterial({ color: 0xffffff });
        const cube = new THREE.Mesh(BoxGeometry, material);
        scene.add(cube);
        cube.position.x = spawnPoints[i].x;
        cube.position.y = spawnPoints[i].y;
        cube.position.z = 0.01;
    }
}

function AddFlag(flagSpawnPoint) {
    let material = new THREE.MeshPhongMaterial({ color: 0x00089d, depthWrite: false });
    let cube = new THREE.Mesh(BoxGeometry, material);
    scene.add(cube);
    cube.position.x = flagSpawnPoint.x;
    cube.position.y = flagSpawnPoint.y;
    cube.position.z = 1;
    cube.scale.x = 0.2;
    cube.scale.z = 0.2;
}
