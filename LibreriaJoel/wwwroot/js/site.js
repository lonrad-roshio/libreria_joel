// Cierra las alertas automáticamente después de unos segundos.
document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        var alertas = document.querySelectorAll('.alert');
        for (var i = 0; i < alertas.length; i++) {
            var alerta = alertas[i];
            var instancia = bootstrap.Alert.getOrCreateInstance(alerta);
            instancia.close();
        }
    }, 5000);
});
