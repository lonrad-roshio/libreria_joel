// Lógica del formulario de registro/edición de ventas: permite agregar varias líneas de
// producto (detalle) y serializa todo como JSON en el campo oculto Venta_DetalleJson,
// tal como lo requiere el modelo Venta (ver Models/Venta.cs).
(function () {
    'use strict';

    function iniciarFormularioVenta(productos, detalleInicial) {
        const cuerpoTabla = document.getElementById('cuerpoDetalleVenta');
        const campoOculto = document.getElementById('Venta_DetalleJson');
        const totalTexto = document.getElementById('totalVenta');
        const btnAgregar = document.getElementById('btnAgregarProducto');

        function recalcularTotal() {
            let total = 0;
            cuerpoTabla.querySelectorAll('tr').forEach(function (fila) {
                const precio = parseFloat(fila.dataset.precio || '0');
                const cantidad = parseFloat(fila.querySelector('.input-cantidad').value || '0');
                const subtotal = precio * cantidad;
                fila.querySelector('.celda-subtotal').textContent = subtotal.toFixed(2);
                total += subtotal;
            });
            totalTexto.textContent = total.toFixed(2);
            sincronizarJson();
        }

        function sincronizarJson() {
            const detalle = [];
            cuerpoTabla.querySelectorAll('tr').forEach(function (fila) {
                const idProducto = parseInt(fila.dataset.idproducto, 10);
                const nombreProducto = fila.dataset.nombre;
                const precio = parseFloat(fila.dataset.precio || '0');
                const cantidad = parseFloat(fila.querySelector('.input-cantidad').value || '0');
                if (cantidad > 0) {
                    detalle.push({
                        idProducto: idProducto,
                        nombreProducto: nombreProducto,
                        cantidad: cantidad,
                        precioUnitario: precio
                    });
                }
            });
            campoOculto.value = JSON.stringify(detalle);
        }

        function agregarFila(idProductoPreseleccionado, cantidadPreseleccionada) {
            const idxSeleccion = productos.findIndex(p => p.idProducto === idProductoPreseleccionado);
            const productoInicial = idxSeleccion >= 0 ? productos[idxSeleccion] : productos[0];
            if (!productoInicial) return;

            const fila = document.createElement('tr');
            fila.dataset.idproducto = productoInicial.idProducto;
            fila.dataset.nombre = productoInicial.nombre;
            fila.dataset.precio = productoInicial.precioVenta;

            const opciones = productos.map(p =>
                `<option value="${p.idProducto}" data-precio="${p.precioVenta}" data-stock="${p.stock}" ${p.idProducto === productoInicial.idProducto ? 'selected' : ''}>${p.nombre} (${p.marca}) - Stock: ${p.stock}</option>`
            ).join('');

            fila.innerHTML = `
                <td>
                    <select class="form-select select-producto">${opciones}</select>
                </td>
                <td><input type="number" class="form-control input-cantidad" min="0.01" step="0.01" value="${cantidadPreseleccionada || 1}" /></td>
                <td class="text-end">Bs. <span class="celda-subtotal">0.00</span></td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-outline-danger btn-quitar">&times;</button>
                </td>`;

            cuerpoTabla.appendChild(fila);

            fila.querySelector('.select-producto').addEventListener('change', function (e) {
                const opt = e.target.selectedOptions[0];
                fila.dataset.idproducto = e.target.value;
                fila.dataset.nombre = opt.textContent;
                fila.dataset.precio = opt.dataset.precio;
                recalcularTotal();
            });

            fila.querySelector('.input-cantidad').addEventListener('input', recalcularTotal);

            fila.querySelector('.btn-quitar').addEventListener('click', function () {
                fila.remove();
                recalcularTotal();
            });

            recalcularTotal();
        }

        btnAgregar.addEventListener('click', function () {
            agregarFila(null, 1);
        });

        if (detalleInicial && detalleInicial.length > 0) {
            detalleInicial.forEach(item => agregarFila(item.idProducto, item.cantidad));
        } else {
            agregarFila(null, 1);
        }

        document.getElementById('formVenta').addEventListener('submit', function (e) {
            sincronizarJson();
            if (cuerpoTabla.querySelectorAll('tr').length === 0) {
                e.preventDefault();
                alert('Debe agregar al menos un producto a la venta.');
            }
        });
    }

    window.iniciarFormularioVenta = iniciarFormularioVenta;
})();
