(function () {
    var maps = [];
    var directionsServices = [];
    var directionsRenderers = [];
    var autoCompletes = [];
    var routeInfos = [];
    var activeInput = null;
    var markers = {}; // Guardará los marcadores de cada mapa

    function initMaps() {
        var mapOptions = {
            center: { lat: 6.25184, lng: -75.5636 },
            zoom: 8
        };

        var geocoder = new google.maps.Geocoder();

        for (var i = 1; i <= 8; i++) {
            let map = new google.maps.Map(document.getElementById('map' + i), mapOptions);
            maps.push(map);

            var directionsService = new google.maps.DirectionsService();
            var directionsRenderer = new google.maps.DirectionsRenderer();
            directionsRenderer.setMap(map);

            directionsServices.push(directionsService);
            directionsRenderers.push(directionsRenderer);

            var options = {
                componentRestrictions: { country: 'CO' }
            };

            var partidaInput = document.getElementById('punto-partida' + (i === 1 ? '' : '-' + i));
            var regresoInput = document.getElementById('punto-regreso' + (i === 1 ? '' : '-' + i));

            var autoCompletePartida = new google.maps.places.Autocomplete(partidaInput, options);
            var autoCompleteRegreso = new google.maps.places.Autocomplete(regresoInput, options);

            autoCompletes.push({ partida: autoCompletePartida, regreso: autoCompleteRegreso });

            routeInfos.push(document.getElementById('informacion-ruta' + i));

            attachAutocompleteListeners(i, autoCompletePartida, autoCompleteRegreso, directionsService, directionsRenderer, routeInfos[i - 1]);

            let markerA = null;
            let markerB = null;

            markers[i] = { A: markerA, B: markerB };

            partidaInput.addEventListener("click", function () {
                activeInput = partidaInput;
            });

            regresoInput.addEventListener("click", function () {
                activeInput = regresoInput;
            });

            // Evento para agregar marcador en el mapa
            map.addListener('click', function (event) {
                if (!activeInput) {
                    alert("Selecciona un campo (Partida o Regreso) antes de hacer clic en el mapa.");
                    return;
                }

                let clickedLocation = event.latLng;
                let isPartida = activeInput === partidaInput;

                if (isPartida) {
                    if (markers[i].A) {
                        markers[i].A.setPosition(clickedLocation);
                    } else {
                        markers[i].A = new google.maps.Marker({
                            position: clickedLocation,
                            map: map,
                            draggable: true // Permitir mover el marcador
                        });

                        // Escuchar evento dragend para actualizar la dirección
                        markers[i].A.addListener('dragend', function () {
                            updateAddress(markers[i].A.getPosition(), partidaInput);
                        });
                    }

                    updateAddress(clickedLocation, partidaInput);
                } else {
                    if (markers[i].B) {
                        markers[i].B.setPosition(clickedLocation);
                    } else {
                        markers[i].B = new google.maps.Marker({
                            position: clickedLocation,
                            map: map,
                            draggable: true
                        });

                        markers[i].B.addListener('dragend', function () {
                            updateAddress(markers[i].B.getPosition(), regresoInput);
                        });
                    }

                    updateAddress(clickedLocation, regresoInput);
                }

                // Si ambos marcadores existen, recalcular la ruta
                if (markers[i].A && markers[i].B) {
                    calculateAndDisplayRoute(i, directionsService, directionsRenderer, markers[i].A.getPosition(), markers[i].B.getPosition(), routeInfos[i - 1]);
                }
            });

            // Agregar marcador en la posición actual
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(function (position) {
                    let currentLocation = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };

                    // Crear un marcador en la posición actual
                    let currentMarker = new google.maps.Marker({
                        position: currentLocation,
                        map: map,
                        title: "Tu ubicación actual",
                        icon: {
                            url: "http://maps.google.com/mapfiles/ms/icons/blue-dot.png" // Marcador azul
                        }
                    });

                    // Centrar el mapa en la posición actual
                    map.setCenter(currentLocation);
                }, function (error) {
                    console.error("Error obteniendo la ubicación: ", error);
                });
            } else {
                console.error("La geolocalización no es compatible con este navegador.");
            }
        }
    }


    function updateAddress(latlng, input) {
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({ location: latlng }, function (results, status) {
            if (status === "OK") {
                if (results[0]) {
                    input.value = results[0].formatted_address;
                } else {
                    input.value = "Dirección no encontrada";
                }
            } else {
                console.error("Error en geocoder: " + status);
            }
        });
    }

    function attachAutocompleteListeners(index, autoCompletePartida, autoCompleteRegreso, directionsService, directionsRenderer, routeInfoDiv) {
        var partidaInput = document.getElementById('punto-partida' + (index === 1 ? '' : '-' + index));
        var regresoInput = document.getElementById('punto-regreso' + (index === 1 ? '' : '-' + index));

        var partidaLocation;
        var regresoLocation;

        autoCompletePartida.addListener('place_changed', function () {
            var place = autoCompletePartida.getPlace();
            if (!place.geometry) return;
            partidaLocation = place.geometry.location;

            if (!markers[index].A) {
                markers[index].A = new google.maps.Marker({
                    position: partidaLocation,
                    map: maps[index - 1],
                    draggable: true
                });

                markers[index].A.addListener('dragend', function () {
                    updateAddress(markers[index].A.getPosition(), partidaInput);
                });
            } else {
                markers[index].A.setPosition(partidaLocation);
            }

            if (partidaLocation && regresoLocation) {
                calculateAndDisplayRoute(index, directionsService, directionsRenderer, partidaLocation, regresoLocation, routeInfoDiv);
            }
        });

        autoCompleteRegreso.addListener('place_changed', function () {
            var place = autoCompleteRegreso.getPlace();
            if (!place.geometry) return;
            regresoLocation = place.geometry.location;

            if (!markers[index].B) {
                markers[index].B = new google.maps.Marker({
                    position: regresoLocation,
                    map: maps[index - 1],
                    draggable: true
                });

                markers[index].B.addListener('dragend', function () {
                    updateAddress(markers[index].B.getPosition(), regresoInput);
                });
            } else {
                markers[index].B.setPosition(regresoLocation);
            }

            if (partidaLocation && regresoLocation) {
                calculateAndDisplayRoute(index, directionsService, directionsRenderer, partidaLocation, regresoLocation, routeInfoDiv);
            }
        });
    }

    function calculateAndDisplayRoute(index, directionsService, directionsRenderer, partidaLocation, regresoLocation, routeInfoDiv) {
        directionsService.route({
            origin: partidaLocation,
            destination: regresoLocation,
            travelMode: 'DRIVING'
        }, function (response, status) {
            if (status === 'OK') {
                directionsRenderer.setDirections(response);
                var leg = response.routes[0].legs[0];
                var distanciaModificada = leg.distance.text.replace(',', '.');
                routeInfoDiv.innerHTML = 'Tiempo estimado: ' + leg.duration.text + '<br>Distancia total: ' + distanciaModificada;
            } else {
                console.error('Error en la ruta: ' + status);
                routeInfoDiv.innerHTML = 'No se pudo calcular la ruta.';
            }
        });
    }


    window.onload = initMaps;
})();
