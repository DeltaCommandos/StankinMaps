let panoramaViewer = null;

document.addEventListener("DOMContentLoaded", function () {
    const mapContainer = document.getElementById("mapContainer");
    const modal = document.getElementById("panoramaModal");
    const closeBtn = document.getElementById("closePanoramaBtn");

    if (!mapContainer || !modal || !closeBtn) {
        return;
    }

    closeBtn.addEventListener("click", closePanorama);

    document.addEventListener("click", async function (event) {
        const panoramaObject = event.target.closest(".panorama-point");

        if (!panoramaObject) {
            return;
        }

        const svgObjectId = panoramaObject.id;
        const building = mapContainer.dataset.building;
        const floor = mapContainer.dataset.floor;

        await openPanoramaByMapObject(building, floor, svgObjectId);
    });
});

//async function openPanoramaByMapObject(building, floor, svgObjectId) {
//    const response = await fetch(`/Panoramas/GetByMapObject?building=${building}&floor=${floor}&svgObjectId=${svgObjectId}`);

//    if (!response.ok) {
//        alert("Для этого объекта панорама пока не добавлена.");
//        return;
//    }

//    const data = await response.json();
//    showPanorama(data);
//}

async function openPanoramaByMapObject(building, floor, svgObjectId) {
    const url = `/Panoramas/GetByMapObject?building=${building}&floor=${floor}&svgObjectId=${svgObjectId}`;

    console.log("Panorama request:", url);

    const response = await fetch(url);

    console.log("Panorama response status:", response.status);

    if (!response.ok) {
        alert(`Для этого объекта панорама пока не добавлена.\n\nЗапрос: ${url}\nСтатус: ${response.status}`);
        return;
    }

    const data = await response.json();

    console.log("Panorama data:", data);

    showPanorama(data);
}



async function openPanoramaById(id) {
    const response = await fetch(`/Panoramas/GetById?id=${id}`);

    if (!response.ok) {
        alert("Панорама не найдена.");
        return;
    }

    const data = await response.json();
    showPanorama(data);
}

function showPanorama(data) {
    const modal = document.getElementById("panoramaModal");
    const title = document.getElementById("panoramaTitle");
    const description = document.getElementById("panoramaDescription");
    const viewerContainer = document.getElementById("panoramaViewer");

    modal.classList.remove("hidden");

    title.textContent = data.title;
    description.textContent = data.description || "";

    if (panoramaViewer) {
        panoramaViewer.destroy();
        panoramaViewer = null;
    }

    const hotSpots = data.hotspots.map(h => {
        return {
            pitch: h.pitch,
            yaw: h.yaw,
            type: "scene",
            text: h.text,
            clickHandlerFunc: function () {
                if (h.targetPanoramaId) {
                    openPanoramaById(h.targetPanoramaId);
                }
            }
        };
    });

    panoramaViewer = pannellum.viewer("panoramaViewer", {
        type: "equirectangular",
        panorama: data.imagePath,
        autoLoad: true,
        yaw: data.defaultYaw,
        pitch: data.defaultPitch,
        hfov: data.defaultHfov,
        hotSpots: hotSpots
    });
}

function closePanorama() {
    const modal = document.getElementById("panoramaModal");

    modal.classList.add("hidden");

    if (panoramaViewer) {
        panoramaViewer.destroy();
        panoramaViewer = null;
    }
}