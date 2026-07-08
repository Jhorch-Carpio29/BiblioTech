document.addEventListener('DOMContentLoaded', () => {
    // 1. Control de sesión y Perfil Dinámico
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return;
    }

    const userInfo = JSON.parse(userInfoStr);

    const headerUserName = document.getElementById('header-user-name');
    const headerUserRole = document.getElementById('header-user-role');
    const profileImg = document.getElementById('header-profile-img');

    if (headerUserName) headerUserName.textContent = userInfo.nombres || 'Admin';
    if (headerUserRole) headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (userInfo.pisoArea || 'Bibliotecario');
    var sidebarUserRole = document.getElementById('sidebar-user-role-text');
    if (sidebarUserRole) { sidebarUserRole.textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (userInfo.pisoArea ? userInfo.pisoArea + ' Personal' : 'UNSCH Library Personal'); }


    if (profileImg) {
        profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres || 'Admin')}&background=random&color=fff&bold=true`;
    }

    // Botón Logout
    const btnLogout = document.getElementById('btn-logout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.href = 'index.html';
        });
    }

    // Modal Events
    const modalNuevoEstudiante = document.getElementById('modal-nuevo-estudiante');
    const btnAbrirModal = document.getElementById('btn-abrir-modal');
    const btnCerrarModal = document.getElementById('btn-cerrar-modal');
    const formNuevoEstudiante = document.getElementById('form-nuevo-estudiante');

    // Función para cargar el select de Escuelas Profesionales
    async function cargarEscuelas() {
        const selectEscuela = document.getElementById('input-escuela');
        if (!selectEscuela) return;
        try {
            const res = await fetch('/api/EscuelasProfesionales');
            if (!res.ok) throw new Error(`HTTP ${res.status}`);
            const escuelas = await res.json();
            selectEscuela.innerHTML = '<option value="">-- Seleccione una escuela --</option>';
            escuelas.forEach(esc => {
                const opt = document.createElement('option');
                opt.value = esc.id;
                opt.textContent = esc.nombre;
                selectEscuela.appendChild(opt);
            });
        } catch (err) {
            console.error('No se pudieron cargar las escuelas:', err);
            selectEscuela.innerHTML = '<option value="">Error al cargar escuelas</option>';
        }
    }

    if (btnAbrirModal) {
        btnAbrirModal.addEventListener('click', () => {
            if (formNuevoEstudiante) {
                formNuevoEstudiante.reset();
                delete formNuevoEstudiante.dataset.mode;
                delete formNuevoEstudiante.dataset.editId;
                const btnSubmit = formNuevoEstudiante.querySelector('button[type="submit"]');
                if (btnSubmit) btnSubmit.textContent = 'GUARDAR ESTUDIANTE';
            }
            cargarEscuelas();
            if (modalNuevoEstudiante) modalNuevoEstudiante.classList.remove('hidden');
        });
    }

    if (btnCerrarModal) {
        btnCerrarModal.addEventListener('click', () => {
            if (modalNuevoEstudiante) modalNuevoEstudiante.classList.add('hidden');
        });
    }

    // 2. Función Leer Estudiantes (GET) y Renderizado Dinámico
    const tableEstudiantes = document.getElementById('table-estudiantes');
    const inputBuscador = document.getElementById('input-buscador');
    let estudiantesCache = [];

    const renderEstudiantes = (estudiantes) => {
        const tbody = document.getElementById('table-estudiantes');
        if (!tbody) return;

        tbody.innerHTML = '';

        if (!estudiantes || estudiantes.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="p-4 text-center">Sin datos registrados</td></tr>';
            return;
        }

        estudiantes.forEach((est) => {
            const tr = document.createElement('tr');
            tr.className = "border-b border-on-surface hover:bg-primary-container/20 transition-colors";

            tr.innerHTML = `
                <td class="p-4 border-r border-on-surface font-bold">${est.dni}</td>
                <td class="p-4 border-r border-on-surface">${est.codigoUniversitario}</td>
                <td class="p-4 border-r border-on-surface">${est.nombresCompletos}</td>
                <td class="p-4 border-r border-on-surface">${est.escuelaNombre || 'S/E'}</td>
                <td class="p-4 border-r border-on-surface text-center">
                    <span class="px-3 py-1 font-label-bold text-label-bold rounded ${est.esMoroso ? 'bg-error text-white' : 'bg-primary text-white'}">
                        ${est.esMoroso ? 'MOROSO' : 'HABILITADO'}
                    </span>
                </td>
                <td class="p-4 text-center">
                    <div class="flex items-center justify-center gap-2">
                        <button type="button" class="btn-editar text-primary hover:text-black transition-colors" title="Editar">
                            <span class="material-symbols-outlined text-[20px]">edit</span>
                        </button>
                        <button type="button" class="btn-eliminar text-error hover:text-black transition-colors" title="Eliminar">
                            <span class="material-symbols-outlined text-[20px]">delete</span>
                        </button>
                    </div>
                </td>
            `;

            const btnEditar = tr.querySelector('.btn-editar');
            const btnEliminar = tr.querySelector('.btn-eliminar');

            if (btnEditar) {
                btnEditar.addEventListener('click', (e) => {
                    e.preventDefault();
                    ejecutarEdicion(est.id);
                });
            }

            if (btnEliminar) {
                btnEliminar.addEventListener('click', (e) => {
                    e.preventDefault();
                    ejecutarEliminacion(est.id);
                });
            }

            tbody.appendChild(tr);
        });
    };

    window.cargarEstudiantes = async function () {
        // Mostrar loading state
        if (tableEstudiantes) {
            tableEstudiantes.innerHTML = '<tr><td colspan="6" class="p-4 text-center text-on-surface-variant"><span class="material-symbols-outlined animate-spin" style="animation: spin 1s linear infinite; display:inline-block;">progress_activity</span> Cargando padrón...</td></tr>';
        }
        try {
            const response = await fetch('/api/Estudiantes');
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            const data = await response.json();
            estudiantesCache = data;
            renderEstudiantes(estudiantesCache);
        } catch (error) {
            console.error('ERROR AL CARGAR:', error);
            if (tableEstudiantes) {
                tableEstudiantes.innerHTML = `<tr><td colspan="6" class="p-4 text-center text-error">Error de conexión con la BD: ${error.message}</td></tr>`;
            }
        }
    };

    if (tableEstudiantes) {
        window.cargarEstudiantes();
    }

    if (inputBuscador) {
        inputBuscador.addEventListener('input', () => {
            const query = inputBuscador.value.trim().toLowerCase();
            if (!query) {
                renderEstudiantes(estudiantesCache);
                return;
            }

            const filtrados = estudiantesCache.filter((estudiante) => {
                const dni = (estudiante.dni || '').toLowerCase();
                const codigo = (estudiante.codigoUniversitario || '').toLowerCase();
                return dni.includes(query) || codigo.includes(query);
            });

            renderEstudiantes(filtrados);
        });
    }

    // 3. Función Guardar Estudiante (Maneja POST para Crear y PUT para Actualizar)
    //    Anti-congelamiento: deshabilita botón durante el fetch
    if (formNuevoEstudiante) {
        formNuevoEstudiante.addEventListener('submit', async (e) => {
            e.preventDefault();

            const btnSubmit = formNuevoEstudiante.querySelector('button[type="submit"]');
            const textoOriginal = btnSubmit ? btnSubmit.textContent : '';

            const inputDni = document.getElementById('input-dni');
            const inputCodigo = document.getElementById('input-codigo');
            const inputNombres = document.getElementById('input-nombres');
            const inputEscuela = document.getElementById('input-escuela');
            const inputMoroso = document.getElementById('input-moroso');

            // Enviamos el objeto con toda la información necesaria
            const estudianteData = {
                id: formNuevoEstudiante.dataset.mode === 'edit' ? parseInt(formNuevoEstudiante.dataset.editId) : 0,
                dni: inputDni ? inputDni.value.trim() : '',
                codigoUniversitario: inputCodigo ? inputCodigo.value.trim() : '',
                nombresCompletos: inputNombres ? inputNombres.value.trim() : '',
                escuelaId: inputEscuela ? parseInt(inputEscuela.value) || 0 : 0,
                esMoroso: inputMoroso ? inputMoroso.checked : false
            };

            const esEdicion = formNuevoEstudiante.dataset.mode === 'edit';
            const idEditado = formNuevoEstudiante.dataset.editId;

            const url = esEdicion
                ? `/api/Estudiantes/${idEditado}`
                : '/api/Estudiantes';

            const metodo = esEdicion ? 'PUT' : 'POST';

            // === ANTI-CONGELAMIENTO ===
            if (btnSubmit) {
                btnSubmit.disabled = true;
                btnSubmit.textContent = 'GUARDANDO...';
            }

            try {
                const response = await fetch(url, {
                    method: metodo,
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(estudianteData)
                });

                if (response.ok) {
                    alert(esEdicion ? 'Estudiante actualizado exitosamente.' : 'Estudiante registrado exitosamente.');

                    formNuevoEstudiante.reset();
                    delete formNuevoEstudiante.dataset.mode;
                    delete formNuevoEstudiante.dataset.editId;

                    if (btnSubmit) btnSubmit.textContent = 'GUARDAR ESTUDIANTE';

                    if (modalNuevoEstudiante) modalNuevoEstudiante.classList.add('hidden');
                    window.cargarEstudiantes();
                } else {
                    const err = await response.text();
                    alert('Error en la operación: ' + err);
                }
            } catch (error) {
                console.error(error);
                alert('Ocurrió un error en la conexión.');
            } finally {
                // === RE-HABILITAR BOTÓN siempre ===
                if (btnSubmit) {
                    btnSubmit.disabled = false;
                    if (btnSubmit.textContent === 'GUARDANDO...') {
                        btnSubmit.textContent = textoOriginal || 'GUARDAR ESTUDIANTE';
                    }
                }
            }
        });
    }

    function ejecutarEdicion(id) {
        const estudiante = estudiantesCache.find(est => est.id == id);
        if (!estudiante) {
            alert('No se encontraron los datos del estudiante.');
            return;
        }

        const inputDni = document.getElementById('input-dni');
        const inputCodigo = document.getElementById('input-codigo');
        const inputNombres = document.getElementById('input-nombres');
        const inputEscuela = document.getElementById('input-escuela');
        const inputMoroso = document.getElementById('input-moroso');

        if (inputDni) inputDni.value = estudiante.dni || '';
        if (inputCodigo) inputCodigo.value = estudiante.codigoUniversitario || '';
        if (inputNombres) inputNombres.value = estudiante.nombresCompletos || '';
        if (inputEscuela) inputEscuela.value = estudiante.escuelaId || '';
        if (inputMoroso) inputMoroso.checked = estudiante.esMoroso || false;

        cargarEscuelas().then(() => {
            const selectEscuela = document.getElementById('input-escuela');
            if (selectEscuela) selectEscuela.value = estudiante.escuelaId || '';
        });

        if (formNuevoEstudiante) {
            formNuevoEstudiante.dataset.mode = 'edit';
            formNuevoEstudiante.dataset.editId = id;

            const btnSubmit = formNuevoEstudiante.querySelector('button[type="submit"]');
            if (btnSubmit) btnSubmit.textContent = 'ACTUALIZAR ESTUDIANTE';
        }

        if (modalNuevoEstudiante) modalNuevoEstudiante.classList.remove('hidden');
    }

    async function ejecutarEliminacion(id) {
        if (!id) {
            alert('ID de estudiante no válido.');
            return;
        }

        if (confirm('¿Seguro que deseas eliminar este estudiante?')) {
            try {
                const response = await fetch(`/api/Estudiantes/${id}`, {
                    method: 'DELETE'
                });
                if (response.ok) {
                    alert('Estudiante eliminado correctamente.');
                    window.cargarEstudiantes();
                } else {
                    alert('No se pudo eliminar el estudiante.');
                }
            } catch (error) {
                console.error(error);
                alert('Error en la conexión al intentar eliminar.');
            }
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
