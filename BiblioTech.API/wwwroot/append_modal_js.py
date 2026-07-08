import os

files = [
    r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\js\admin_dashboard.js',
    r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\js\gestion_personal.js'
]

modal_logic = r'''
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
'''

for p in files:
    if not os.path.exists(p): continue
    with open(p, 'r', encoding='utf-8') as f:
        c = f.read()

    # Avoid duplicate injection
    if 'LÓGICA DE MI PERFIL' in c:
        continue

    idx = c.rfind('});')
    if idx != -1:
        c = c[:idx] + modal_logic + '\n' + c[idx:]
        
    with open(p, 'w', encoding='utf-8') as f:
        f.write(c)

print('Appended modal logic to admin JS files.')
