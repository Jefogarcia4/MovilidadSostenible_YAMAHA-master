(function () {
    let originAutocompletes = [];
    let destinationAutocompletes = [];
    let distanceOutputs = [];

    function initAutocomplete() {
        for (let i = 1; i <= 6; i++) {
            originAutocompletes[i] = new google.maps.places.Autocomplete(
                document.getElementById(`origin-input-${i}`),
                { types: ['geocode'] }
            );

            destinationAutocompletes[i] = new google.maps.places.Autocomplete(
                document.getElementById(`destination-input-${i}`),
                { types: ['geocode'] }
            );

            destinationAutocompletes[i].addListener('place_changed', () => calculateDistance(i));
        }
    }

    function calculateDistance(trayectoIndex) {
        const origin = originAutocompletes[trayectoIndex].getPlace();
        const destination = destinationAutocompletes[trayectoIndex].getPlace();

        if (!origin || !destination) {
            document.getElementById(`distance-output-${trayectoIndex}`).textContent = 'Por favor seleccione un origen y un destino';
            return;
        }

        const originLatLng = new google.maps.LatLng(origin.geometry.location.lat(), origin.geometry.location.lng());
        const destinationLatLng = new google.maps.LatLng(destination.geometry.location.lat(), destination.geometry.location.lng());

        const distanceService = new google.maps.DistanceMatrixService();
        distanceService.getDistanceMatrix({
            origins: [originLatLng],
            destinations: [destinationLatLng],
            travelMode: 'DRIVING'
        }, (response, status) => {
            if (status === 'OK') {
                const element = response.rows[0].elements[0];
                const distance = element.distance.text;
                const duration = element.duration.text;
                document.getElementById(`distance-output-${trayectoIndex}`).textContent = `Tiempo estimado: ${duration}, Distancia total: ${distance}`;
            } else {
                document.getElementById(`distance-output-${trayectoIndex}`).textContent = 'Error calculando el tiempo y la distancia: ' + status;
            }
        });
    }

    window.onload = initAutocomplete;
})();
