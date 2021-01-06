const Width = window.innerWidth;
const Height = window.innerHeight - 56;

import * as THREE from '../js/lib/three.module.js';
//import { OrbitControls } from "../js/lib/three/jsm/controls/OrbitControls.js";

const renderer = new THREE.WebGLRenderer();
renderer.setSize(Width, Height);
document.body.appendChild(renderer.domElement).id = 'game';;

const camera = new THREE.PerspectiveCamera(75, Width / Height, 0.1, 50);
camera.rotation.x = -Math.PI / 2
camera.position.y = 10;
camera.position.x = 6;
camera.position.z = 6;

//const controls = new OrbitControls(camera, renderer.domElement);
//controls.update();

const scene = new THREE.Scene();
const BoxGeometry = new THREE.BoxGeometry();

document.body.style.marginBottom = 0;

const hemiLight = new THREE.HemisphereLight(0xffffff, 0x444444);
hemiLight.position.set(0, 20, 0);
scene.add(hemiLight);

export function animate() {
    requestAnimationFrame(animate);
    //controls.update();
    renderer.render(scene, camera);
}


export function drawMap(map) {
    let Cell = {
        None: 0,
        Empty: 1,
        Wall: 2,
        Digging: 3
    }

    for (var x = 0; x < map.width; x++) {
        for (var z = 0; z < map.height; z++) {
            if (map.map[x * map.width + z] != Cell.None) {
                let material = new THREE.MeshPhongMaterial({ color: 0xffffff });
                if (map.map[x * map.width + z] == Cell.Empty) {
                    material = new THREE.MeshPhongMaterial({ color: 0x464646 });
                } else if (map.map[x * map.width + z] == Cell.Wall) {
                    material = new THREE.MeshPhongMaterial({ color: 0x2ce900 });
                } else if (map.map[x * map.width + z] == Cell.Digging) {
                    material = new THREE.MeshPhongMaterial({ color: 0xeb0034 });
                }
                const cube = new THREE.Mesh(BoxGeometry, material);
                scene.add(cube);
                cube.position.x = x;
                cube.position.z = z;
            }
        }
    }

    for (var i = 0; i < map.spawnPoints.length; i++) {
        let material = new THREE.MeshPhongMaterial({ color: 0xffffff });
        const cube = new THREE.Mesh(BoxGeometry, material);
        scene.add(cube);
        cube.position.x = map.spawnPoints[i].x;
        cube.position.z = map.spawnPoints[i].y;
        cube.position.y = 0.01;
    }

    let material = new THREE.MeshPhongMaterial({ color: 0x00089d, depthWrite: false });
    let cube = new THREE.Mesh(BoxGeometry, material);
    scene.add(cube);
    cube.position.x = map.flagSpawnPoint.x;
    cube.position.z = map.flagSpawnPoint.y;
    cube.position.y = 1;
    cube.scale.x = 0.2;
    cube.scale.z = 0.2;


}
