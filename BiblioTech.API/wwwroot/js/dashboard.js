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
    var sidebarUserRole = document.getElementById('sidebar-user-role-text');
    if (sidebarUserRole) { sidebarUserRole.textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (userInfo.pisoArea ? userInfo.pisoArea + ' Personal' : 'UNSCH Library Personal'); }




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
            const ahora = new Date();

            // Contar préstamos según su estado
            const activos = prestamos.filter(p => p.estado !== 'Devuelto').length;
            
            // Devoluciones hoy
            const devolucionesHoy = prestamos.filter(p => {
                if (p.estado !== 'Devuelto' || !p.fechaHoraDevolucion) return false;
                const fDev = new Date(p.fechaHoraDevolucion);
                return fDev.toDateString() === ahora.toDateString();
            }).length;

            if (countActivosEl) countActivosEl.textContent = activos;
            if (countDevolucionesEl) countDevolucionesEl.textContent = devolucionesHoy;

            // Actualizar tabla con los movimientos recientes
            if (tableBody) {
                tableBody.innerHTML = '';
                const recientes = prestamos.slice(-5).reverse();

                if (recientes.length === 0) {
                    tableBody.innerHTML = '<tr><td colspan="5" class="p-4 text-center">No hay movimientos recientes</td></tr>';
                } else {
                    recientes.forEach(p => {
                        const tr = document.createElement('tr');
                        tr.className = 'hover:bg-primary-container transition-colors group';

                        // Evaluar estado visual (si está vencido)
                        let estadoVisual = p.estado;
                        if (p.estado !== 'Devuelto' && new Date(p.fechaHoraLimite) < ahora) {
                            estadoVisual = 'Vencido';
                        }

                        let badgeClass = estadoVisual === 'Devuelto' ? 'bg-primary-container text-on-surface' 
                                    : estadoVisual === 'Vencido' ? 'bg-error text-on-error'
                                    : 'bg-tertiary-fixed text-on-surface';

                        tr.innerHTML = `
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface font-bold">${p.id.substring(0,8)}</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface font-bold group-hover:underline">${p.libroTitulo || 'S/T'}</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface">${p.estudianteNombre || 'S/N'}</td>
                            <td class="p-4 border-b-border-width border-r-border-width border-on-surface">${estadoVisual === 'Devuelto' ? 'Devolución' : 'Salida'}</td>
                            <td class="p-4 border-b-border-width border-on-surface">
                                <span class="${badgeClass} font-label-bold text-table-header border-2 border-on-surface px-3 py-1 uppercase inline-block">${estadoVisual}</span>
                            </td>
                        `;
                        tableBody.appendChild(tr);
                    });
                }
            }
        }

        // Obtener Libros para la tarjeta "SIN STOCK"
        const librosResponse = await fetch('/api/Libros');
        if (librosResponse.ok) {
            const libros = await librosResponse.json();
            const sinStock = libros.filter(l => l.stockDisponible <= 0).length;
            if (countVencidosEl) countVencidosEl.textContent = sinStock;
        }

    } catch (error) {
        console.error('Error de conexión con la API', error);

        if (countActivosEl) countActivosEl.textContent = 'Err';
        if (countDevolucionesEl) countDevolucionesEl.textContent = 'Err';
        if (countVencidosEl) countVencidosEl.textContent = 'Err';

        if (tableBody) {
            tableBody.innerHTML = '<tr><td colspan="5" class="p-4 text-center text-error border-b-border-width border-on-surface">Error al cargar datos</td></tr>';
        }
    }

    // ==========================================
    // LÓGICA DE MI PERFIL (CONFIGURACIÓN)
    // ==========================================
    const modalPerfil = document.getElementById('modal-perfil');
    const btnCerrarPerfil = document.getElementById('btn-cerrar-perfil');
    const formPerfil = document.getElementById('form-perfil');
    const perfilLoading = document.getElementById('perfil-loading');
    const btnGuardarPerfil = document.getElementById('btn-guardar-perfil');
    const btnProfileDropdown = document.getElementById('btn-profile-dropdown');
    const btnConfiguracion = document.getElementById('btn-configuracion');

    function abrirModalPerfil() {
        if (!modalPerfil) return;
        modalPerfil.classList.remove('hidden');
        perfilLoading.classList.remove('hidden');
        formPerfil.classList.add('hidden');
        btnGuardarPerfil.classList.add('hidden');

        fetch('/api/Personal/profile/' + userInfo.id)
            .then(res => {
                if (!res.ok) throw new Error('Error al cargar perfil');
                return res.json();
            })
            .then(data => {
                document.getElementById('perfil-dni').value = data.dni || '';
                document.getElementById('perfil-nombres').value = data.nombres || '';
                document.getElementById('perfil-email').value = data.correo || '';
                document.getElementById('perfil-area').value = data.pisoArea || '';
                
                perfilLoading.classList.add('hidden');
                formPerfil.classList.remove('hidden');
                btnGuardarPerfil.classList.remove('hidden');
            })
            .catch(err => {
                console.error(err);
                alert('No se pudo cargar la información del perfil.');
                modalPerfil.classList.add('hidden');
            });
    }

    if (btnProfileDropdown) btnProfileDropdown.addEventListener('click', abrirModalPerfil);
    if (btnConfiguracion) btnConfiguracion.addEventListener('click', abrirModalPerfil);

    if (btnCerrarPerfil) {
        btnCerrarPerfil.addEventListener('click', () => {
            modalPerfil.classList.add('hidden');
        });
    }

    if (formPerfil) {
        formPerfil.addEventListener('submit', (e) => {
            e.preventDefault();
            const nombres = document.getElementById('perfil-nombres').value.trim();
            const correo = document.getElementById('perfil-email').value.trim();
            const pisoArea = document.getElementById('perfil-area').value.trim();

            if (!nombres || !correo) {
                alert('Nombres y Correo son obligatorios.');
                return;
            }

            btnGuardarPerfil.disabled = true;
            btnGuardarPerfil.innerHTML = '<span class="material-symbols-outlined animate-spin">progress_activity</span> Guardando...';

            fetch('/api/Personal/profile/' + userInfo.id, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ nombres, correo, pisoArea })
            })
            .then(res => {
                if (!res.ok) throw new Error('Error al guardar perfil');
                alert('¡Perfil actualizado con éxito!');
                
                userInfo.nombres = nombres;
                userInfo.pisoArea = pisoArea;
                localStorage.setItem('usuarioInfo', JSON.stringify(userInfo));
                
                const headerUserName = document.getElementById('header-user-name');
                const headerUserRole = document.getElementById('header-user-role');
                const profileImg = document.getElementById('header-profile-img');

                if (headerUserName) headerUserName.textContent = nombres;
                if (headerUserRole) headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (pisoArea || 'Bibliotecario');
                var sidebarUserRole = document.getElementById('sidebar-user-role-text');
                if (sidebarUserRole) { sidebarUserRole.textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (pisoArea ? pisoArea + ' Personal' : 'UNSCH Library Personal'); }

                if (profileImg) {
                    profileImg.src = 'https://ui-avatars.com/api/?name=' + encodeURIComponent(nombres) + '&background=random&color=fff&bold=true';
                }

                modalPerfil.classList.add('hidden');
            })
            .catch(err => {
                console.error(err);
                alert('No se pudo guardar la información del perfil.');
            })
            .finally(() => {
                btnGuardarPerfil.disabled = false;
                btnGuardarPerfil.innerHTML = '<span class="material-symbols-outlined">save</span> Guardar Cambios';
            });
        });
    }
});
