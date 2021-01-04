const Width = window.innerWidth;
const Height = window.innerHeight - 56;

import * as THREE from '../js/lib/three.module.js';

const renderer = new THREE.WebGLRenderer();
renderer.setSize(Width, Height);
document.body.appendChild(renderer.domElement).id = 'game';;

const camera = new THREE.PerspectiveCamera(75, Width / Height, 0.1, 50);
camera.rotation.x = -Math.PI / 2;
camera.position.y = 10;
camera.position.x = 6;
camera.position.z = 6;

const scene = new THREE.Scene();
const geometry = new THREE.BoxGeometry();

document.body.style.marginBottom = 0;

export function animate() {
    requestAnimationFrame(animate);

    renderer.render(scene, camera);
}


export function drawMap(map) {
    let Cell = {
        None: 0,
        Empty: 1,
        Wall: 2,
        Digging: 3,
        Flag: 4
    }

    for (var x = 0; x < map.width; x++) {
        for (var z = 0; z < map.height; z++) {
            if (map.map[x * map.width + z] != Cell.None) {
                let material;
                if (map.map[x * map.width + z] == Cell.Empty) {
                    material = new THREE.MeshBasicMaterial({ color: 0x464646 });
                } else if (map.map[x * map.width + z] == Cell.Wall) {
                    material = new THREE.MeshBasicMaterial({ color: 0x2ce900 });
                } else if (map.map[x * map.width + z] == Cell.Digging) {
                    material = new THREE.MeshBasicMaterial({ color: 0xeb0034 });
                } else if (map.map[x * map.width + z] == Cell.Flag) {
                    material = new THREE.MeshBasicMaterial({ color: 0x00089d });
                }
                const cube = new THREE.Mesh(geometry, material);
                scene.add(cube);
                cube.position.x = x;
                cube.position.z = z;
            }
        }
    }
}
