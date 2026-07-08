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
    
    const sidebarUserRoleLoad = document.getElementById('sidebar-user-role-text');
    if (sidebarUserRoleLoad) {
        sidebarUserRoleLoad.querySelector('span').textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (userInfo.pisoArea ? userInfo.pisoArea + ' Admin' : 'UNSCH Admin');
    }
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
                    <div class="p-4 flex items-center justify-center gap-2">
                        <button class="btn-edit-user w-8 h-8 border-2 border-on-background bg-surface hover:bg-primary-container flex items-center justify-center transition-colors group-hover:scale-110" data-user='${JSON.stringify(u)}'>
                            <span class="material-symbols-outlined text-sm">edit</span>
                        </button>
                        <button class="btn-delete-user w-8 h-8 border-2 border-on-background bg-surface hover:bg-error-container text-on-surface hover:text-error flex items-center justify-center transition-colors group-hover:scale-110" data-id="${u.id}">
                            <span class="material-symbols-outlined text-sm">delete</span>
                        </button>
                    </div>
                `;
                tablePersonal.appendChild(row);
            });
        } catch (err) {
            console.error(err);
        }
    };

    // Event Delegation for Edit/Delete
    const tablePersonal = document.getElementById('table-personal');
    if (tablePersonal) {
        tablePersonal.addEventListener('click', async (e) => {
            const btnDelete = e.target.closest('.btn-delete-user');
            const btnEdit = e.target.closest('.btn-edit-user');

            if (btnDelete) {
                const id = btnDelete.getAttribute('data-id');
                if (confirm('¿Estás seguro de eliminar este usuario del sistema?')) {
                    try {
                        const res = await fetch('/api/Usuarios/' + id, { method: 'DELETE' });
                        if (res.ok) {
                            alert('Usuario eliminado exitosamente');
                            await loadPersonal();
                        } else {
                            alert('No se pudo eliminar el usuario.');
                        }
                    } catch(err) { console.error(err); }
                }
            }

            if (btnEdit) {
                const uStr = btnEdit.getAttribute('data-user');
                if(uStr) {
                    const u = JSON.parse(uStr);
                    window.editingUserId = u.id;
                    document.getElementById('input-dni').value = u.dni;
                    document.getElementById('input-nombres').value = u.nombres;
                    document.getElementById('input-correo').value = u.email;
                    document.getElementById('input-password').value = ''; // Se ignora en update
                    document.getElementById('input-rol').value = u.rolId;
                    document.getElementById('input-piso').value = u.pisoArea;

                    const submitBtn = document.querySelector('#form-nuevo-personal button[type="submit"]');
                    if(submitBtn) {
                        submitBtn.innerHTML = '<span class="material-symbols-outlined">save_as</span> Actualizar Personal';
                    }
                    
                    document.getElementById('form-nuevo-personal').scrollIntoView({behavior: 'smooth', block: 'start'});
                }
            }
        });
    }

    await loadPersonal();

    // Registro o Actualización
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

            const isEdit = !!window.editingUserId;
            const url = isEdit ? '/api/Usuarios/' + window.editingUserId : '/api/Usuarios';
            const method = isEdit ? 'PUT' : 'POST';

            const payload = isEdit ? {
                dni: dni,
                nombres: nombres,
                email: email,
                pisoArea: pisoArea,
                rolId: rolId,
                activo: true
            } : {
                dni: dni,
                nombres: nombres,
                email: email,
                passwordHash: password,
                pisoArea: pisoArea,
                rolId: rolId
            };

            try {
                const response = await fetch(url, {
                    method: method,
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (response.ok) {
                    alert(isEdit ? 'Personal actualizado exitosamente' : 'Personal registrado exitosamente');
                    formNuevoPersonal.reset();
                    window.editingUserId = null;
                    const submitBtn = document.querySelector('#form-nuevo-personal button[type="submit"]');
                    if(submitBtn) submitBtn.innerHTML = '<span class="material-symbols-outlined text-[20px]" style="font-variation-settings: \'FILL\' 1;">add</span> Añadir Personal';
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


    // ==========================================
    // LÓGICA DE MI PERFIL (CONFIGURACIÓN)
    // ==========================================
    const modalPerfil = document.getElementById('modal-perfil');
    const btnCerrarPerfil = document.getElementById('btn-cerrar-perfil');
    const formPerfil = document.getElementById('form-perfil');
    const perfilLoading = document.getElementById('perfil-loading');
    const btnGuardarPerfil = document.getElementById('btn-guardar-perfil');
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
            btnGuardarPerfil.innerHTML = '<span class="material-symbols-outlined animate-spin" style="animation: spin 1s linear infinite;">progress_activity</span> Guardando...';

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
                
                const headerUserName = document.getElementById('header-user-name') || document.querySelector('header .text-right p:first-child');
                const headerUserRole = document.getElementById('header-user-role') || document.querySelector('header .text-right p:last-child');
                const profileImg = document.getElementById('header-profile-img') || document.querySelector('header img');
                const sidebarUserRole = document.getElementById('sidebar-user-role-text');

                if (headerUserName) headerUserName.textContent = nombres;
                if (headerUserRole) headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (pisoArea || 'Admin');
                if (sidebarUserRole) { sidebarUserRole.querySelector('span').textContent = userInfo.rolId === 1 ? 'UNSCH Admin' : (pisoArea ? pisoArea + ' Admin' : 'UNSCH Admin'); }

                if (profileImg) {
                    profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(nombres)}&background=random&color=fff&bold=true`;
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
