import sys
import re
import os

def fix_file(filepath):
    print(f"Processing {filepath}...")
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # We want to find where the injected block is:
    # "    // ==========================================\n    // L"
    idx = content.find('    // ==========================================')
    if idx == -1:
        print(f'Not found in {filepath}')
        return
        
    # The pattern we want to remove is the injected block, which ends with "});\n        });\n    }\n"
    # Or just use regex to match the block:
    # We injected: \n    // ========================================== ... 
    # until the final `});`
    pattern = re.compile(r'\n    // ==========================================.*?\n    \}\n\}\);\n', re.DOTALL)
    match = pattern.search(content)
    if not match:
        print(f'Pattern end not found in {filepath}')
        # Maybe it doesn't end with exactly that?
        # Let's just find the end of the btnGuardarPerfil block
        pattern2 = re.compile(r'\n    // ==========================================.*?Guardar Cambios\';\n            \}\);\n        \}\);\n    \}\n', re.DOTALL)
        match = pattern2.search(content)
        if not match:
            print("Fallback pattern failed too.")
            return

    # Clean content: remove the block, and insert the original "});" for fetch
    clean_content = content[:match.start()] + '\n            });\n' + content[match.end():]
    
    # Now find the end of DOMContentLoaded (the last `});` before window.eliminar... or similar)
    m2 = re.search(r'\}\);\n+(?:// Función|window\.)', clean_content)
    if m2:
        pos = m2.start()
        
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

        fetch(`/api/Personal/profile/` + userInfo.id)
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

            fetch(`/api/Personal/profile/` + userInfo.id, {
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
"""
        final_content = clean_content[:pos] + logic + '\n' + clean_content[pos:]
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(final_content)
        print(f'Successfully fixed {filepath}')
    else:
        print(f'Could not find DOMContentLoaded end in {filepath}')

for f in ['inventario.js', 'estudiantes.js', 'prestamos.js']:
    fix_file('d:/UNSCH/400 I/Pruebas y Aseguramiento de Calidad de Software/LaboratorioZapata/EXAMEN 01/SISTEMA DE BIBLIOTECA/BiblioTech/BiblioTech.API/wwwroot/js/' + f)
