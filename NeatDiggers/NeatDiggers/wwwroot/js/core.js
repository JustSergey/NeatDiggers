import * as THREE from '../lib/three/build/three.module.js';
import * as actions from "./actions.js";
import { GLTFLoader } from '../lib/three/examples/jsm/loaders/GLTFLoader.js';
import { OrbitControls } from '../lib/three/examples/jsm/controls/OrbitControls.js';


export let controls;
let  camera, renderer;
export let mapArray, scene, sFlag, sPlayer, sPlayers = new THREE.Group();
let pandora, jupiter, box_1, box_2, box_3, box_4, spawn, dig_1, dig_2, flag;
const loader = new GLTFLoader();

export let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        this.width = window.innerWidth;
        this.height = window.innerHeight - $('header').outerHeight();
        renderer = new THREE.WebGLRenderer();
        renderer.shadowMap.enabled = true;
        renderer.shadowMap.type = THREE.PCFSoftShadowMap;
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
    cameraInit(new THREE.Vector3(map.width / 2, map.height / 2, 1));
    sceneInit(new THREE.Vector3(map.width / 2, map.height / 2, 1));
    drawMap(map);
    animate();
    actions.setCamera(camera);
}

function animate() {
    requestAnimationFrame(animate);
    controls.update();
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
        let player = getPlayer(players[i].id);
        if (player == null) {
            let material = new THREE.MeshPhongMaterial({ color: 0x6cc924 });
            let cube = new THREE.Mesh(boxGeometry, material)
            switch (players[i].character.name) {
                case 1: cube = pandora.clone(); break;
                case 3: cube = jupiter.clone(); break;
            }

            cube.position.set(players[i].position.x, players[i].position.y, 1);
            cube.info = players[i];
            sPlayers.add(cube);
            if (players[i].id == userId) {
                sPlayer = cube;
                material.color.set(0x15c194);
            }
        }
        else {
            player.info = players[i]; 
            player.position.set(players[i].position.x, players[i].position.y, players[i].position.z);
        }
    }
}

//export function moveFlag(pos) {
//    util.move(sFlag, pos);
//}

async function drawMap(map) {
    await loadModels();
    mapArray = map;
    drawFloor(map);
    drawSpawnPoints(map.spawnPoints);
    drawFlag(map.flagSpawnPoint);
}

async function loadModels() {
    pandora = await modelLoader("pandora");
    pandora.castShadow = true;
    pandora.receiveShadow = true;
    jupiter = await modelLoader("jupiter");
    jupiter.castShadow = true;
    jupiter.receiveShadow = true;
    flag = await modelLoader("flag");
    flag.castShadow = true;
    flag.receiveShadow = true;
    box_1 = await modelLoader("box_1");
    box_1.receiveShadow = true;
    box_2 = await modelLoader("box_2");
    box_2.receiveShadow = true;
    box_3 = await modelLoader("box_3");
    box_3.receiveShadow = true;
    box_4 = await modelLoader("box_4");
    box_4.receiveShadow = true;
    spawn = await modelLoader("spawn");
    spawn.receiveShadow = true;
    dig_1 = await modelLoader("dig_1");
    dig_1.receiveShadow = true;
    dig_2 = await modelLoader("dig_2");
    dig_2.receiveShadow = true;


}

function drawFlag(pos) {
    sFlag = flag.clone();
    scene.add(sFlag);
    sFlag.position.set(pos.x, pos.y, 1);
}

function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}

function drawFloor(map) {
    const Cell = {
        None: 0,
        Empty: 1,
        Wall: 2,
        Digging: 3
    };

    const boxGeometry = new THREE.BoxGeometry();
    let materialWall = new THREE.MeshPhongMaterial({ color: 0x2ce900 });

    for (var x = 0; x < map.width; x++) {
        for (var y = 0; y < map.height; y++) {
            if (map.map[x * map.width + y] != Cell.None) {
                let cube;
                switch (map.map[x * map.width + y]) {
                    case Cell.Empty:
                        switch (getRandomInt(4)) {
                            case 0: cube = box_1.clone(); break;
                            case 1: cube = box_2.clone(); break;
                            case 2: cube = box_3.clone(); break;
                            case 3: cube = box_4.clone(); break;
                        }
                        break;
                    case Cell.Wall: cube = new THREE.Mesh(boxGeometry, materialWall); break;
                    case Cell.Digging:
                        switch (getRandomInt(2)) {
                            case 0: cube = dig_1.clone(); break;
                            case 1: cube = dig_2.clone(); break;
                        }
                        break;
                }
                cube.rotation.y = Math.PI / 2 * getRandomInt(4);
                scene.add(cube);
                cube.position.set(x, y);
            }
        }
    }
}

function drawSpawnPoints(spawnPoints) {
    for (var i = 0; i < spawnPoints.length; i++) {
        let cube = spawn.clone();
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

function cameraInit(target) {
    camera = new THREE.PerspectiveCamera(75, screen.width / screen.height, 0.1, 50);
    camera.position.set(target.x, target.y, 10);
    camera.up.set(0, 0, 1);
    controls = new OrbitControls(camera, renderer.domElement);

    controls.enableDamping = true;
    controls.dampingFactor = 0.05;
    //controls.screenSpacePanning = false;
    controls.minDistance = 5;
    controls.maxDistance = 15;
    controls.target = target;
    controls.maxPolarAngle = Math.PI / 2;
    controls.update();
}

function sceneInit(target) {
    scene = new THREE.Scene();
    scene.fog = new THREE.FogExp2(0x000000, 0.02);

    const dirLight1 = new THREE.DirectionalLight(0xffffff);
    dirLight1.position.set(target.x * 2, target.y * 2, 20);
    dirLight1.shadow.camera.far = 50;
    dirLight1.castShadow = true;

    const targetObject = new THREE.Object3D();
    scene.add(targetObject);
    targetObject.position.x = target.x;
    targetObject.position.y = target.y;

    dirLight1.target = targetObject;

    var side = 10;
    dirLight1.shadow.camera.top = side;
    dirLight1.shadow.camera.bottom = -side;
    dirLight1.shadow.camera.left = side;
    dirLight1.shadow.camera.right = -side;
    scene.add(dirLight1);

    const dirLight2 = new THREE.DirectionalLight(0x002288);
    dirLight2.position.set(- 1, - 1, - 1);
    dirLight2.castShadow = true;
    scene.add(dirLight2);

    const ambientLight = new THREE.AmbientLight(0x555555);
    scene.add(ambientLight);
    scene.add(sPlayers);}

function modelLoader(name) {
    return new Promise((resolve, reject) => {
        loader.load("../../StaticFiles/models/" + name + ".glb", data => resolve(data.scene.children[0]), null, reject);
    });
}

function getPlayer(userId) {
    for (var i = 0; i < sPlayers.children.length; i++)
        if (sPlayers.children[i].info.id == userId)
            return sPlayers.children[i];
    return null;
}