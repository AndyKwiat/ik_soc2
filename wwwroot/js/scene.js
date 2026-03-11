window.scene3d = {
    renderer: null,
    scene: null,
    camera: null,
    controls: null,

    init: function (canvasId) {
        const canvas = document.getElementById(canvasId);

        // Renderer
        this.renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
        this.renderer.setSize(canvas.clientWidth, canvas.clientHeight);
        this.renderer.setPixelRatio(window.devicePixelRatio);
        this.renderer.setClearColor(0x1a1a2e);

        // Scene
        this.scene = new THREE.Scene();

        // Camera
        this.camera = new THREE.PerspectiveCamera(
            60,
            canvas.clientWidth / canvas.clientHeight,
            0.1,
            1000
        );
        this.camera.position.set(3, 2, 4);

        // Orbit controls
        this.controls = new THREE.OrbitControls(this.camera, canvas);
        this.controls.enableDamping = true;
        this.controls.dampingFactor = 0.08;
        this.controls.target.set(0, 0, 0);

        // Lights
        const ambient = new THREE.AmbientLight(0xffffff, 0.6);
        this.scene.add(ambient);
        const directional = new THREE.DirectionalLight(0xffffff, 0.8);
        directional.position.set(5, 10, 7);
        this.scene.add(directional);

        // Grid helper for orientation
        const grid = new THREE.GridHelper(10, 10, 0x444466, 0x333355);
        this.scene.add(grid);

        // Handle resize
        const self = this;
        window.addEventListener("resize", function () {
            const w = canvas.clientWidth;
            const h = canvas.clientHeight;
            self.camera.aspect = w / h;
            self.camera.updateProjectionMatrix();
            self.renderer.setSize(w, h);
        });

        // Start render loop
        this._animate();
    },

    addLine: function (x1, y1, z1, x2, y2, z2, colorHex) {
        const material = new THREE.LineBasicMaterial({ color: colorHex, linewidth: 2 });
        const geometry = new THREE.BufferGeometry().setFromPoints([
            new THREE.Vector3(x1, y1, z1),
            new THREE.Vector3(x2, y2, z2),
        ]);
        const line = new THREE.Line(geometry, material);
        this.scene.add(line);
    },

    addNode: function (x, y, z, colorHex, size) {
        const geometry = new THREE.BoxGeometry(size, size, size);
        const material = new THREE.MeshPhongMaterial({ color: colorHex });
        const mesh = new THREE.Mesh(geometry, material);
        mesh.position.set(x, y, z);
        this.scene.add(mesh);
    },

    clearScene: function () {
        const toRemove = [];
        this.scene.traverse(function (obj) {
            if (obj.type === "Line" || obj.type === "Mesh") {
                toRemove.push(obj);
            }
        });
        for (const obj of toRemove) {
            obj.geometry.dispose();
            obj.material.dispose();
            this.scene.remove(obj);
        }
    },

    registerKeyHandler: function (dotnetRef) {
        window.addEventListener("keydown", function (e) {
            var handled = ["ArrowUp", "ArrowDown", "ArrowLeft", "ArrowRight", "u", "d", "U", "D"];
            if (handled.indexOf(e.key) !== -1) {
                e.preventDefault();
                dotnetRef.invokeMethodAsync("OnKeyPress", e.key);
            }
        });
    },

    _animate: function () {
        const self = this;
        function loop() {
            requestAnimationFrame(loop);
            self.controls.update();
            self.renderer.render(self.scene, self.camera);
        }
        loop();
    },
};
