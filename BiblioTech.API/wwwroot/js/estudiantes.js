document.addEventListener('DOMContentLoaded', () => {
    // 1. Control de sesión y Perfil Dinámico
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return;
    }

    const userInfo = JSON.parse(userInfoStr);
    
    // Header Info
    const headerUserName = document.getElementById('header-user-name');
    const headerUserRole = document.getElementById('header-user-role');
    const profileImg = document.getElementById('header-profile-img');
    
    if (headerUserName) {
        headerUserName.textContent = userInfo.nombres || 'Admin';
    }
    if (headerUserRole) {
        headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : 'Personal';
    }
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

    if (btnAbrirModal) {
        btnAbrirModal.addEventListener('click', () => {
            if (modalNuevoEstudiante) modalNuevoEstudiante.classList.remove('hidden');
        });
    }
    if (btnCerrarModal) {
        btnCerrarModal.addEventListener('click', () => {
            if (modalNuevoEstudiante) modalNuevoEstudiante.classList.add('hidden');
        });
    }

    // 2. Función Leer Estudiantes (GET)
    const tableEstudiantes = document.getElementById('table-estudiantes');
    const inputBuscador = document.getElementById('input-buscador');
    let estudiantesCache = [];

    const renderEstudiantes = (estudiantes) => {
        const tbody = document.getElementById('table-estudiantes');
        if (!tbody) {
            return;
        }

        tbody.innerHTML = '';
        estudiantes.forEach(est => {
            tbody.innerHTML += `
                <tr>
                    <td>${est.dni || ''}</td>
                    <td>${est.codigoUniversitario || ''}</td>
                    <td>${est.nombres || ''}</td>
                    <td>${est.escuelaProfesional || ''}</td>
                    <td>${est.esMoroso ? '<span class="badge bg-danger">MOROSO</span>' : '<span class="badge bg-success">HABILITADO</span>'}</td>
                </tr>
            `;
        });
    };

    window.cargarEstudiantes = async function() {
        try {
            const response = await fetch('/api/Estudiantes');
            if (!response.ok) throw new Error('Error al cargar estudiantes');
            const data = await response.json();
            estudiantesCache = Array.isArray(data) ? data : [];
            renderEstudiantes(estudiantesCache);
        } catch (error) {
            console.error(error);
            if (tableEstudiantes) {
                tableEstudiantes.innerHTML = '<tr><td colspan="6" class="p-4 text-center text-on-surface font-bold">Error cargando el padrón de estudiantes</td></tr>';
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

    // 3. Función Registrar Estudiante (POST)
    const formNuevoEstudiante = document.getElementById('form-nuevo-estudiante');
    if (formNuevoEstudiante) {
        formNuevoEstudiante.addEventListener('submit', async (e) => {
            e.preventDefault();

            const inputDni = document.getElementById('input-dni');
            const inputCodigo = document.getElementById('input-codigo');
            const inputNombres = document.getElementById('input-nombres');
            const inputEscuela = document.getElementById('input-escuela');

            // Asumimos que los nombres se ingresaron completos en un solo input para simplificar (o divídelo en nombres/apellidos según API)
            // Se envía un objeto compatible con lo general
            const nuevoEstudiante = {
                dni: inputDni ? inputDni.value.trim() : '',
                codigoUniversitario: inputCodigo ? inputCodigo.value.trim() : '',
                nombres: inputNombres ? inputNombres.value.trim() : '',
                escuelaProfesional: inputEscuela ? inputEscuela.value.trim() : '',
                esMoroso: false
            };

            try {
                const response = await fetch('/api/Estudiantes', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(nuevoEstudiante)
                });

                if (response.ok) {
                    alert('Estudiante registrado exitosamente.');
                    formNuevoEstudiante.reset();
                    if(modalNuevoEstudiante) modalNuevoEstudiante.classList.add('hidden');
                    if(window.cargarEstudiantes) window.cargarEstudiantes();
                } else {
                    const err = await response.text();
                    alert('Error al registrar estudiante: ' + err);
                }
            } catch (error) {
                console.error(error);
                alert('Ocurrió un error en la conexión.');
            }
        });
    }
});

// 4. Función Eliminar Estudiante (DELETE) global
window.eliminarEstudiante = async function(id) {
    if (confirm('¿Seguro que deseas eliminar este estudiante?')) {
        try {
            const response = await fetch(`/api/Estudiantes/${id}`, {
                method: 'DELETE'
            });
            if (response.ok) {
                alert('Estudiante eliminado correctamente.');
                if(window.cargarEstudiantes) window.cargarEstudiantes();
            } else {
                alert('No se pudo eliminar el estudiante.');
            }
        } catch (error) {
            console.error(error);
            alert('Error en la conexión.');
        }
    }
};