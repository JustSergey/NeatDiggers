import * as THREE from '../lib/three/build/three.module.js';
import * as actions from "./actions.js";
import { OrbitControls } from '../lib/three/examples/jsm/controls/OrbitControls.js';
import { modelLoader } from './util.js';


export let controls;
let  camera, renderer;
export let mapArray, scene, sFlag, sPlayer, sPlayers = new THREE.Group();
let models = {
    pandora: null,
    jupiter: null,
    box_1: null,
    box_2: null,
    box_3: null,
    box_4: null,
    spawn: null,
    dig_1: null,
    dig_2: null,
    flag: null
}


let centerMap;

export let screen = {
    width: 0,
    height: 0,
    'resize': function () {
        document.body.style.overflow = "hidden";

        this.width = window.innerWidth;
        this.height = window.innerHeight - $('header').outerHeight();
        renderer = new THREE.WebGLRenderer({ canvas: $("#game")[0]});
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

export async function init(map) {
    renderInit();
    centerMap = new THREE.Vector3((map.width / 2) - 0.5, (map.height / 2) - 0.5, 1);
    cameraInit(centerMap);
    sceneInit(centerMap);
    await drawMap(map);
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
                case 1: cube = models.pandora.clone(); break;
                case 3: cube = models.jupiter.clone(); break;
            }

            cube.position.set(players[i].position.x, players[i].position.y, 1);
            cube.info = players[i];
            sPlayers.add(cube);
            if (players[i].id == userId) {
                sPlayer = cube;

                camera.position.x = sPlayer.position.x;
                camera.position.y = sPlayer.position.y;
            }
            cube.up.set(0, 0, 1);
            cube.lookAt(centerMap);
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
    models.pandora = await modelLoader("pandora");
    models.pandora.castShadow = true;
    models.pandora.receiveShadow = true;
    models.jupiter = await modelLoader("jupiter");
    models.jupiter.castShadow = true;
    models.jupiter.receiveShadow = true;

    models.flag = await modelLoader("flag");
    models.flag.castShadow = true;
    models.flag.receiveShadow = true;
    models.box_1 = await modelLoader("box_1");
    models.box_1.receiveShadow = true;
    models.box_2 = await modelLoader("box_2");
    models.box_2.receiveShadow = true;
    models.box_3 = await modelLoader("box_3");
    models.box_3.receiveShadow = true;
    models.box_4 = await modelLoader("box_4");
    models.box_4.receiveShadow = true;
    models.spawn = await modelLoader("spawn");
    models.spawn.receiveShadow = true;
    models.dig_1 = await modelLoader("dig_1");
    models.dig_1.receiveShadow = true;
    models.dig_2 = await modelLoader("dig_2");
    models.dig_2.receiveShadow = true;
}

function drawFlag(pos) {
    sFlag = models.flag.clone();
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
                            case 0: cube = models.box_1.clone(); break;
                            case 1: cube = models.box_2.clone(); break;
                            case 2: cube = models.box_3.clone(); break;
                            case 3: cube = models.box_4.clone(); break;
                        }
                        break;
                    case Cell.Wall: cube = new THREE.Mesh(boxGeometry, materialWall); break;
                    case Cell.Digging:
                        switch (getRandomInt(2)) {
                            case 0: cube = models.dig_1.clone(); break;
                            case 1: cube = models.dig_2.clone(); break;
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
        let cube = models.spawn.clone();
        scene.add(cube);
        cube.position.set(spawnPoints[i].x, spawnPoints[i].y, 0.01);
    }
}

function renderInit() {
    screen.resize();
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



function getPlayer(userId) {
    for (var i = 0; i < sPlayers.children.length; i++)
        if (sPlayers.children[i].info.id == userId)
            return sPlayers.children[i];
    return null;
}