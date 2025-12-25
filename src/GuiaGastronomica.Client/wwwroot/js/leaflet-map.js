// leaflet-map.js - Interop entre Blazor y Leaflet.js

let map;
let markers = [];
let markersByVenue = new Map();

export function initializeMap(lat, lng, venuesJson) {
    // Esperar a que el div tenga tamaño
    const mapDiv = document.getElementById('map');
    if (!mapDiv) return;
    let retries = 0;
    function tryInit() {
        const rect = mapDiv.getBoundingClientRect();
        if (rect.width < 50 || rect.height < 50) {
            if (retries++ < 20) {
                setTimeout(tryInit, 100);
                return;
            }
        }
        // Crear mapa centrado en Huelva
        map = window.L.map('map').setView([lat, lng], 13);

        // Agregar capa base (OpenStreetMap)
        window.L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
            maxZoom: 19
        }).addTo(map);

        // Parsear venues desde JSON
        const venues = JSON.parse(venuesJson);
        
        // Agregar markers para cada venue
        addMarkersToMap(venues);
    }
    tryInit();
}

function addMarkersToMap(venues) {
    // Limpiar markers anteriores
    markers.forEach(marker => map.removeLayer(marker));
    markers = [];
    markersByVenue.clear();

    // Crear markers
    venues.forEach(venue => {
        const icon = window.L.circleMarker([venue.latitude, venue.longitude], {
            radius: 8,
            fillColor: venue.color,
            color: "#000",
            weight: 1,
            opacity: 1,
            fillOpacity: 0.8
        });

        const popupContent = `
            <div style="width: 200px;">
                <h4 style="margin: 0 0 8px 0;">${venue.name}</h4>
                <p style="margin: 4px 0;"><strong>Zona:</strong> ${venue.zone}</p>
                <p style="margin: 4px 0;"><strong>Rating:</strong> ${venue.score.toFixed(1)} ⭐</p>
                <a href="/venue/${venue.id}" style="color: #1976d2; text-decoration: none;">Ver detalles →</a>
            </div>
        `;

        icon.bindPopup(popupContent);
        icon.addTo(map);

        markers.push(icon);
        markersByVenue.set(venue.id, icon);
    });
}

export function updateMapMarkers(venuesJson) {
    const venues = JSON.parse(venuesJson);
    addMarkersToMap(venues);
    
    // Ajustar vista al area de venues filtrados
    if (venues.length > 0) {
        const bounds = window.L.latLngBounds(venues.map(v => [v.latitude, v.longitude]));
        map.fitBounds(bounds, { padding: [50, 50] });
    }
}

export function centerMapOnVenue(lat, lng) {
    map.setView([lat, lng], 16, { animate: true });
}

export function highlightVenue(venueId) {
    const marker = markersByVenue.get(venueId);
    if (marker) {
        marker.openPopup();
        marker.setStyle({ radius: 12, weight: 2 });
    }
}

export function unhighlightVenue(venueId) {
    const marker = markersByVenue.get(venueId);
    if (marker) {
        marker.closePopup();
        marker.setStyle({ radius: 8, weight: 1 });
    }
}
