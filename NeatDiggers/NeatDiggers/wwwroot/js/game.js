const Width = window.innerWidth;
const Height = window.innerHeight - 56;

import * as THREE from '../js/lib/three.module.js';
//import * as DAT from '../js/lib/dat.gui.module.js';

//let camera_gui = {
//	posy: 10,
//}

//const gui = new DAT.GUI();
//gui.add(camera_gui, 'posy').min(5).max(50).step(1)

const renderer = new THREE.WebGLRenderer();
renderer.setSize(Width, Height);
document.body.appendChild(renderer.domElement).id = 'game';;

const camera = new THREE.PerspectiveCamera(75, Width / Height, 0.1, 50);
camera.rotation.x = -Math.PI / 2;
camera.position.y = 10;

const scene = new THREE.Scene();
const geometry = new THREE.BoxGeometry();
const material = new THREE.MeshBasicMaterial({ color: 0x00ff00 });
const cube = new THREE.Mesh(geometry, material);
scene.add(cube);
document.body.style.marginBottom = 0;

export function animate(map) {
	requestAnimationFrame(animate);

	cube.rotation.z += 0.01;
	cube.rotation.y += 0.01;

	renderer.render(scene, camera);
}

