document.addEventListener('DOMContentLoaded', async () => {
    // Control de sesión
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return; // Detener ejecución
    }

    const userInfo = JSON.parse(userInfoStr);

    // Rellenar cabecera de perfil en la vista
    const headerUserName = document.getElementById('header-user-name');
    const headerUserRole = document.getElementById('header-user-role');

    if (headerUserName) {
        headerUserName.textContent = userInfo.nombres;
    }

    const profileImg = document.querySelector('img[alt="Librarian Profile"]');
    if (profileImg && userInfo.nombres) {
        profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres)}&background=random&color=fff&bold=true`;
    }

    if (headerUserRole) {
        // En caso de Admin o info de rol en el objeto
        headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (userInfo.pisoArea || 'Bibliotecario');
    }

    // Fecha actual
    const dateEl = document.getElementById('current-date');
    if (dateEl) {
        const options = { weekday: 'long', day: 'numeric', month: 'short' };
        let dateStr = new Intl.DateTimeFormat('es-ES', options).format(new Date());
        dateStr = dateStr.charAt(0).toUpperCase() + dateStr.slice(1);
        dateEl.textContent = dateStr;
    }

    // Referencias a los elementos del DOM (dashboard.html)
    const countActivosEl = document.getElementById('count-activos');
    const countDevolucionesEl = document.getElementById('count-devoluciones');
    const countVencidosEl = document.getElementById('count-vencidos');
    const tableBody = document.getElementById('table-movimientos');

    try {
        // Obtener Préstamos
        const prestamosResponse = await fetch('/api/Prestamos');
        if (prestamosResponse.ok) {
            const prestamos = await prestamosResponse.json();

            // Contar préstamos según su estado
            const activos = prestamos.filter(p => p.estado === 'Activo' || p.estado === 'Vigente').length;
            const devueltos = prestamos.filter(p => p.estado === 'Devuelto').length;
            const vencidos = prestamos.filter(p => p.estado === 'Vencido').length;

            if (countActivosEl) countActivosEl.textContent = activos;
            if (countDevolucionesEl) countDevolucionesEl.textContent = devueltos;
            if (countVencidosEl) countVencidosEl.textContent = vencidos;

            // Actualizar tabla con los movimientos recientes
            if (tableBody) {
                tableBody.innerHTML = '';

                const recientes = prestamos.slice(-5).reverse();

                if (recientes.length === 0) {
                    tableBody.innerHTML = '<tr><td colspan="5" class="p-4 text-center">No hay préstamos recientes</td></tr>';
                } else {
                    recientes.forEach(p => {
                        const tr = document.createElement('tr');
                        tr.className = 'hover:bg-primary-container transition-colors group';

                        let badgeClass = p.estado === 'Activo' ? 'bg-tertiary-fixed text-on-surface' 
                                    : p.estado === 'Devuelto' ? 'bg-primary-container text-on-surface'
                                    : 'bg-error text-on-error';

                        tr.innerHTML = `
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface font-bold">${p.id.substring(0,8)}...</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface font-bold group-hover:underline">${p.libroTitulo || 'Desconocido'}</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface">${p.estudianteNombre || 'Desconocido'}</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface">${p.estado === 'Devuelto' ? 'Devo' : 'Salida'}</td>
                            <td class="p-4 border-b-border-width border-on-surface">
                                <span class="${badgeClass} font-label-bold text-table-header border-2 border-on-surface px-3 py-1 uppercase inline-block">${p.estado}</span>
                            </td>
                        `;
                        tableBody.appendChild(tr);
                    });
                }
            }
        } else {
            console.error('Error al obtener préstamos');
        }

        // Obtener Libros (si aún es necesario en otras partes de la UI se pueden usar, aunque dashboard.html usa Préstamos)
        // const librosResponse = await fetch('/api/Libros');

    } catch (error) {
        console.error('Error de conexión con la API', error);

        // Mostrar mensaje visual si falla
        if (countActivosEl) countActivosEl.textContent = 'Err';
        if (countDevolucionesEl) countDevolucionesEl.textContent = 'Err';
        if (countVencidosEl) countVencidosEl.textContent = 'Err';

        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="5" class="p-4 text-center text-error border-b-border-width border-on-surface">Error al cargar datos</td></tr>';
        }
    }
});
