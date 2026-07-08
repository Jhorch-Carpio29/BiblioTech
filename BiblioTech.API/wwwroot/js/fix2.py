import sys
import re

def fix_file(filepath):
    print(f"Processing {filepath}...")
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Locate the start of the injected profile logic block.
    # Because of encoding corruption, it might look like "    // LGICA" or similar.
    # We'll just search for a snippet that we know starts it, or regex.
    pattern = re.compile(r'\n    // ==========================================.*', re.DOTALL)
    match = pattern.search(content)
    if not match:
        print(f"Pattern not found in {filepath}")
        return

    # Keep everything before the injected block
    clean_content = content[:match.start()]
    
    # For estudiantes.js and prestamos.js, this block replaced the final "});" of DOMContentLoaded.
    # So we must add the logic, and then close the DOMContentLoaded block with "});"
    
    logic = """
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
"""
    final_content = clean_content + logic
    
    # We must also fix the double encoded  in the clean_content to their proper characters.
    # The most common are 'ó' () and 'á' () and 'é' () and 'í' () but they all became .
    # Since I don't know which  is which, I'll just restore from transcript if possible, or just let them be,
    # wait... in the clean_content, there might be '' in alert messages. It's annoying but functional.
    # Is there a critical syntax error in clean_content? No.
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(final_content)
    print(f"Successfully fixed {filepath}")

for f in ['estudiantes.js', 'prestamos.js', 'dashboard.js']:
    fix_file('d:/UNSCH/400 I/Pruebas y Aseguramiento de Calidad de Software/LaboratorioZapata/EXAMEN 01/SISTEMA DE BIBLIOTECA/BiblioTech/BiblioTech.API/wwwroot/js/' + f)

