// Cierra las alertas automáticamente después de unos segundos.
document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        document.querySelectorAll('.alert').forEach(function (alerta) {
            var instancia = bootstrap.Alert.getOrCreateInstance(alerta);
            instancia.close();
        });
    }, 5000);
});
