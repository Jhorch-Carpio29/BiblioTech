document.addEventListener('DOMContentLoaded', async () => {
    console.log("Script cargado correctamente");

    // Validar Sesión
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return;
    }

    const userInfo = JSON.parse(userInfoStr);

    // Inyectar perfil en el header
    const profileName = document.querySelector('header .text-right p:first-child');
    const profileRole = document.querySelector('header .text-right p:last-child');
    const profileImg = document.querySelector('header img');

    if (profileName) profileName.textContent = userInfo.nombres;
    if (profileRole) profileRole.textContent = userInfo.rolId === 1 ? 'Administrador del Sistema' : 'Personal de Biblioteca';
    if (profileImg) profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres)}&background=random&color=fff&size=150`;

    // Logout
    const btnLogout = document.getElementById('btn-logout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.removeItem('usuarioInfo');
            window.location.href = 'index.html';
        });
    }

    const loadPersonal = async () => {
        try {
            const tablePersonal = document.getElementById('table-personal');
            if (!tablePersonal) return;

            const res = await fetch('/api/Usuarios');
            if (!res.ok) throw new Error('Error al cargar personal');
            
            const usuarios = await res.json();

            // Limpiar filas de la tabla
            tablePersonal.innerHTML = '';

            if (usuarios.length === 0) {
                tablePersonal.innerHTML = '<div class="p-4 text-center font-bold">No hay personal registrado</div>';
                return;
            }

            usuarios.forEach(u => {
                const row = document.createElement('div');
                row.className = 'grid grid-cols-[100px_minmax(200px,1fr)_minmax(200px,1fr)_120px_180px_100px_80px] border-b-4 border-on-background hover:bg-surface-variant transition-colors group cursor-pointer';
                
                const rolName = u.rolId === 1 ? 'Admin' : 'Personal';
                const statusName = u.activo ? 'Activo' : 'Inactivo';
                const statusColor = u.activo ? 'bg-primary-fixed' : 'bg-error';

                row.innerHTML = `
                    <div class="p-4 font-body-md text-body-md border-r-4 border-on-background flex items-center font-bold">${u.dni || 'Sin DNI'}</div>
                    <div class="p-4 font-body-md text-body-md border-r-4 border-on-background flex items-center uppercase">${u.nombres}</div>
                    <div class="p-4 font-body-md text-body-md border-r-4 border-on-background flex items-center">${u.email}</div>
                    <div class="p-4 border-r-4 border-on-background flex items-center">
                        <span class="bg-secondary-fixed border-2 border-on-background px-2 py-1 font-label-bold text-label-bold text-[10px] uppercase w-full text-center">${rolName}</span>
                    </div>
                    <div class="p-4 font-body-md text-body-md border-r-4 border-on-background flex items-center">${u.pisoArea || 'General'}</div>
                    <div class="p-4 border-r-4 border-on-background flex items-center">
                        <div class="w-4 h-4 rounded-full border-2 border-on-background ${statusColor}"></div>
                        <span class="ml-2 font-label-bold text-label-bold text-xs uppercase">${statusName}</span>
                    </div>
                    <div class="p-4 flex items-center justify-center">
                        <button class="w-8 h-8 border-2 border-on-background bg-surface hover:bg-secondary-container flex items-center justify-center transition-colors group-hover:scale-110">
                            <span class="material-symbols-outlined text-sm">edit</span>
                        </button>
                    </div>
                `;
                tablePersonal.appendChild(row);
            });
        } catch (err) {
            console.error(err);
        }
    };

    await loadPersonal();

    // Registro
    const formNuevoPersonal = document.getElementById('form-nuevo-personal');
    const formError = document.getElementById('form-error');

    if (formNuevoPersonal) {
        formNuevoPersonal.addEventListener('submit', async (e) => {
            e.preventDefault();
            if (formError) {
                formError.classList.add('hidden');
                formError.textContent = '';
            }

            const dni = document.getElementById('input-dni').value;
            const nombres = document.getElementById('input-nombres').value;
            const email = document.getElementById('input-correo').value;
            const password = document.getElementById('input-password').value;
            const rolId = parseInt(document.getElementById('input-rol').value, 10);
            const pisoArea = document.getElementById('input-piso').value;

            try {
                const response = await fetch('/api/Usuarios', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        dni: dni,
                        nombres: nombres,
                        email: email,
                        passwordHash: password,
                        pisoArea: pisoArea,
                        rolId: rolId
                    })
                });

                if (response.ok) {
                    alert('Personal registrado exitosamente');
                    formNuevoPersonal.reset();
                    await loadPersonal();
                } else {
                    const errMsg = await response.text();
                    if (formError) {
                        formError.textContent = errMsg || 'Error al registrar personal.';
                        formError.classList.remove('hidden');
                    } else {
                        alert(errMsg || 'Error al registrar personal.');
                    }
                }
            } catch (err) {
                console.error(err);
                if (formError) {
                    formError.textContent = 'Error de red al conectar con el servidor.';
                    formError.classList.remove('hidden');
                } else {
                    alert('Error de red al conectar con el servidor.');
                }
            }
        });
    }

});
