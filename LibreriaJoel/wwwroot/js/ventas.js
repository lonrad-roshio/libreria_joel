// Lógica del formulario de registro/edición de ventas: permite agregar varias líneas de
// producto y prepara el detalle como campos de formulario para el modelo Venta.
(function () {
    'use strict';

    function iniciarFormularioVenta(detalleInicial) {
        const cuerpoTabla = document.getElementById('cuerpoDetalleVenta');
        const totalTexto = document.getElementById('totalVenta');
        const btnAgregar = document.getElementById('btnAgregarProducto');
        const opcionesProductos = Array.from(
            document.getElementById('productosDisponibles').options
        );
        const productos = opcionesProductos.map(function (opcion) {
            return {
                idProducto: Number(opcion.value),
                nombre: opcion.dataset.nombre,
                marca: opcion.dataset.marca,
                precioVenta: Number(opcion.dataset.precio),
                stock: Number(opcion.dataset.stock)
            };
        });

        function recalcularTotal() {
            let total = 0;
            const filas = cuerpoTabla.querySelectorAll('tr');
            for (let i = 0; i < filas.length; i++) {
                const fila = filas[i];
                const precio = parseFloat(fila.dataset.precio || '0');
                const campoCantidad = fila.querySelector('.input-cantidad');
                const cantidad = parseFloat(campoCantidad.value || '0');
                const subtotal = precio * cantidad;
                const celdaSubtotal = fila.querySelector('.celda-subtotal');
                celdaSubtotal.textContent = subtotal.toFixed(2);
                total += subtotal;
            }
            totalTexto.textContent = total.toFixed(2);
            sincronizarCampos();
        }

        function sincronizarCampos() {
            const filas = cuerpoTabla.querySelectorAll('tr');
            for (let i = 0; i < filas.length; i++) {
                const fila = filas[i];
                const campoProducto = fila.querySelector('.input-idproducto');
                const campoPrecio = fila.querySelector('.input-precio');
                const campoCantidad = fila.querySelector('.input-cantidad');
                campoProducto.name = `Venta.Detalles[${i}].IdProducto`;
                campoPrecio.name = `Venta.Detalles[${i}].PrecioUnitario`;
                campoCantidad.name = `Venta.Detalles[${i}].Cantidad`;
                campoProducto.value = fila.dataset.idproducto;
                campoPrecio.value = fila.dataset.precio;
            }
        }

        function agregarFila(idProductoPreseleccionado, cantidadPreseleccionada) {
            let idxSeleccion = -1;
            for (let i = 0; i < productos.length; i++) {
                if (productos[i].idProducto === idProductoPreseleccionado) {
                    idxSeleccion = i;
                    break;
                }
            }

            const productoInicial = idxSeleccion >= 0
                ? productos[idxSeleccion]
                : productos.find(producto => producto.stock > 0);
            if (!productoInicial) return;

            const fila = document.createElement('tr');
            fila.dataset.idproducto = productoInicial.idProducto;
            fila.dataset.precio = productoInicial.precioVenta;

            let opciones = '';
            for (let i = 0; i < productos.length; i++) {
                const producto = productos[i];
                const seleccionado = producto.idProducto === productoInicial.idProducto ? 'selected' : '';
                opciones += `<option value="${producto.idProducto}" data-precio="${producto.precioVenta}" data-stock="${producto.stock}" ${seleccionado}>${producto.nombre} (${producto.marca}) - Stock: ${producto.stock}</option>`;
            }

            fila.innerHTML = `
                <td>
                    <input type="hidden" class="input-idproducto" />
                    <input type="hidden" class="input-precio" />
                    <select class="form-select select-producto">${opciones}</select>
                </td>
                <td><input type="number" class="form-control input-cantidad" min="0.01" step="0.01" value="${cantidadPreseleccionada || 1}" /></td>
                <td class="text-end">Bs. <span class="celda-subtotal">0.00</span></td>
                <td class="text-center">
                    <button type="button" class="btn btn-sm btn-outline-danger btn-quitar">&times;</button>
                </td>`;

            cuerpoTabla.appendChild(fila);

            const selectorProducto = fila.querySelector('.select-producto');
            selectorProducto.addEventListener('change', function (e) {
                const opt = e.target.selectedOptions[0];
                fila.dataset.idproducto = e.target.value;
                fila.dataset.precio = opt.dataset.precio;
                recalcularTotal();
            });

            const campoCantidad = fila.querySelector('.input-cantidad');
            campoCantidad.addEventListener('input', recalcularTotal);

            const botonQuitar = fila.querySelector('.btn-quitar');
            botonQuitar.addEventListener('click', function () {
                fila.remove();
                recalcularTotal();
            });

            recalcularTotal();
        }

        btnAgregar.addEventListener('click', function () {
            agregarFila(null, 1);
        });

        if (detalleInicial && detalleInicial.length > 0) {
            for (let i = 0; i < detalleInicial.length; i++) {
                const item = detalleInicial[i];
                agregarFila(item.idProducto, item.cantidad);
            }
        } else {
            agregarFila(null, 1);
        }

        const formulario = document.getElementById('formVenta');
        formulario.addEventListener('submit', function (e) {
            sincronizarCampos();
            const filas = cuerpoTabla.querySelectorAll('tr');
            if (filas.length === 0) {
                e.preventDefault();
                alert('Debe agregar al menos un producto a la venta.');
            }
        });
    }

    window.iniciarFormularioVenta = iniciarFormularioVenta;
})();
