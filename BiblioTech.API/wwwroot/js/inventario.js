document.addEventListener('DOMContentLoaded', () => {
    // 1. Control de sesión y Perfil Dinámico
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return;
    }

    const userInfo = JSON.parse(userInfoStr);
    
    // Header
    const headerUserName = document.getElementById('header-user-name');
    const headerUserRole = document.getElementById('header-user-role');
    const profileImg = document.getElementById('header-profile-img');
    
    if (headerUserName) {
        headerUserName.textContent = userInfo.nombres || 'Admin';
        
        if (profileImg) {
            profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres || 'Admin')}&background=random&color=fff&bold=true`;
        }
    }
    
    if (headerUserRole) {
        headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (userInfo.pisoArea || 'Bibliotecario');
    }
    var sidebarUserRole = document.getElementById('sidebar-user-role-text');
    if (sidebarUserRole) { sidebarUserRole.textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (userInfo.pisoArea ? userInfo.pisoArea + ' Personal' : 'UNSCH Library Personal'); }

    // 2. Salir
    const btnLogout = document.getElementById('btn-logout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.href = 'index.html';
        });
    }

    // Modal Drawer logic
    const modalNuevoLibro = document.getElementById('modal-nuevo-libro');
    const btnAbrirModal = document.getElementById('btn-abrir-modal');
    const btnCerrarModal = document.getElementById('btn-cerrar-modal');

    if (btnAbrirModal) {
        btnAbrirModal.addEventListener('click', () => {
            modalNuevoLibro.classList.remove('hidden');
        });
    }

    if (btnCerrarModal) {
        btnCerrarModal.addEventListener('click', () => {
            modalNuevoLibro.classList.add('hidden');
        });
    }

    // 3. GET Libros (Lectura)
    const tableLibros = document.getElementById('table-libros');

    window.cargarLibros = async function() {
        try {
            const response = await fetch('/api/Libros');
            if (!response.ok) throw new Error('Error al cargar libros');
            const data = await response.json();

            tableLibros.innerHTML = ''; // Limpiar

            data.forEach(libro => {
                const tr = document.createElement('tr');
                tr.className = 'border-b-border-width border-on-surface hover:bg-surface-container transition-colors';

                // Usamos las propiedades correctas
                const isbn = libro.codigoIsbn || libro.isbn || 'N/A';
                const stockDisponible = libro.stockDisponible !== undefined ? libro.stockDisponible : libro.stockTotal;
                let stockBadgeClass = stockDisponible > 0 ? 'bg-success text-on-surface' : 'bg-danger text-on-error';

                // Mapear el nombre de la categoría o usar ID
                let nombreCategoria = 'General';
                if (libro.categoria && libro.categoria.nombre) {
                    nombreCategoria = libro.categoria.nombre;
                } else if (libro.categoriaId) {
                    nombreCategoria = 'ID: ' + libro.categoriaId;
                } else if (typeof libro.categoria === 'string') {
                    nombreCategoria = libro.categoria;
                }

                tr.innerHTML = `
                    <td class="p-4 border-r-border-width border-on-surface font-bold text-on-surface">${libro.titulo || 'Sin título'}</td>
                    <td class="p-4 border-r-border-width border-on-surface text-on-surface-variant">${libro.autor || 'Sin autor'}</td>
                    <td class="p-4 border-r-border-width border-on-surface font-mono text-sm text-on-surface">${isbn}</td>
                    <td class="p-4 border-r-border-width border-on-surface">
                        <span class="px-3 py-1 bg-surface-variant border-2 border-on-surface font-label-bold text-[10px] uppercase text-on-surface">${nombreCategoria}</span>
                    </td>
                    <td class="p-4 border-r-border-width border-on-surface text-center font-bold text-on-surface">${libro.stockTotal || 0}</td>
                    <td class="p-4 border-r-border-width border-on-surface text-center font-bold">
                        <span class="px-3 py-1 border-2 border-on-surface font-label-bold text-[12px] uppercase ${stockBadgeClass}">${stockDisponible}</span>
                    </td>
                    <td class="p-4 text-center">
                        <div class="flex justify-center gap-3">
                            <button class="hover:text-primary transition-colors disabled:opacity-50" title="Editar"><span class="material-symbols-outlined text-[20px] text-on-surface">edit</span></button>
                            <button onclick="eliminarLibro(${libro.id})" class="hover:text-error transition-colors disabled:opacity-50" title="Eliminar"><span class="material-symbols-outlined text-[20px] text-on-surface">delete</span></button>
                        </div>
                    </td>
                `;
                tableLibros.appendChild(tr);
            });
        } catch (error) {
            console.error(error);
            tableLibros.innerHTML = '<tr><td colspan="7" class="p-4 text-center text-on-surface font-bold">Error cargando el catálogo</td></tr>';
        }
    }

    if (tableLibros) {
        window.cargarLibros();
    }

    // 4. POST Libros (Registro)
    const formNuevoLibro = document.getElementById('form-nuevo-libro');
    if (formNuevoLibro) {
        formNuevoLibro.addEventListener('submit', async (e) => {
            e.preventDefault();

            const inputTitulo = document.getElementById('input-titulo');
            const inputAutor = document.getElementById('input-autor');
            const inputIsbn = document.getElementById('input-isbn');
            const inputCategoria = document.getElementById('input-categoria');
            const inputStock = document.getElementById('input-stock');

            const nuevoLibro = {
                titulo: inputTitulo ? inputTitulo.value.trim() : '',
                autor: inputAutor ? inputAutor.value.trim() : '',
                isbn: inputIsbn ? inputIsbn.value.trim() : '',
                fechaPublicacion: new Date().toISOString().split('T')[0],
                categoriaId: inputCategoria ? (parseInt(inputCategoria.value.trim()) || 1) : 1, 
                stockTotal: inputStock ? parseInt(inputStock.value.trim(), 10) : 0,
                stockDisponible: inputStock ? parseInt(inputStock.value.trim(), 10) : 0
            };

            try {
                const response = await fetch('/api/Libros', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(nuevoLibro)
                });

                if (response.ok) {
                    alert('Libro registrado exitosamente.');
                    formNuevoLibro.reset();
                    if(modalNuevoLibro) modalNuevoLibro.classList.add('hidden');
                    if(window.cargarLibros) window.cargarLibros(); // Refrescar la tabla
                } else {
                    const err = await response.text();
                    alert('Error al registrar el libro: ' + err);
                }
            } catch (error) {
                console.error(error);
                alert('Ocurrió un error en la conexión.');
            }
        });
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

// Función Eliminar Global
window.eliminarLibro = async function(id) {
    if (confirm('¿Seguro que deseas eliminar este libro?')) {
        try {
            const response = await fetch(`/api/Libros/${id}`, {
                method: 'DELETE'
            });
            if (response.ok) {
                alert('Libro eliminado correctamente.');
                if(window.cargarLibros) window.cargarLibros();
            } else {
                alert('No se pudo eliminar el libro.');
            }
        } catch (error) {
            console.error(error);
            alert('Error en la conexión.');
        }
    }
};
