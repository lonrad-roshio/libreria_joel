// Calcula automáticamente el "Precio de venta" a partir del "Costo de entrada" y un
// porcentaje de ganancia que ingresa el usuario (ej. 30%). El campo de precio de venta
// se deja editable en todo momento para que el usuario pueda redondear el resultado
// (que puede salir con varios decimales) o ajustarlo manualmente si lo desea.
(function () {
    'use strict';

    function iniciarCalculoPrecioVenta(idCosto, idMargen, idPrecioVenta) {
        const inputCosto = document.getElementById(idCosto);
        const inputMargen = document.getElementById(idMargen);
        const inputPrecio = document.getElementById(idPrecioVenta);

        if (!inputCosto || !inputMargen || !inputPrecio) return;

        function recalcular() {
            const costo = parseFloat(inputCosto.value);
            const margen = parseFloat(inputMargen.value);

            if (isNaN(costo) || isNaN(margen)) return;

            const precioSugerido = costo * (1 + margen / 100);
            // Redondeado a 2 decimales como sugerencia; el usuario puede volver a editarlo.
            inputPrecio.value = precioSugerido.toFixed(2);
        }

        inputCosto.addEventListener('input', recalcular);
        inputMargen.addEventListener('input', recalcular);
    }

    window.iniciarCalculoPrecioVenta = iniciarCalculoPrecioVenta;
})();
