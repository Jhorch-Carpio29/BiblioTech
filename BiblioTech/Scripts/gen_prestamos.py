import os

js_code = """document.addEventListener('DOMContentLoaded', () => {
    // ==========================================
    // 1. CONTROL DE SESIÓN Y PERFIL DINÁMICO
    // ==========================================
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
    
    if (profileImg) {
        profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres || 'Admin')}&background=random&color=fff&bold=true`;
    }

    const btnLogout = document.getElementById('btn-logout');
    if (btnLogout) {
        btnLogout.addEventListener('click', (e) => {
            e.preventDefault();
            localStorage.clear();
            window.location.href = 'index.html';
        });
    }

    // Configurar min datetime
    const inputDatetime = document.getElementById('datetime');
    if (inputDatetime) {
        const ahora = new Date();
        ahora.setMinutes(ahora.getMinutes() - ahora.getTimezoneOffset());
        inputDatetime.min = ahora.toISOString().slice(0, 16);
    }

    // ==========================================
    // 2. LÓGICA DE PRÉSTAMOS
    // ==========================================
    const formPrestamo = document.getElementById('form-prestamo');
    const contenedorPrestamos = document.getElementById('contenedor-prestamos');

    async function buscarEstudiante(dni) {
        const res = await fetch('/api/Estudiantes');
        if (!res.ok) throw new Error('Error al cargar estudiantes');
        const estudiantes = await res.json();
        return estudiantes.find(e => e.dni === dni);
    }

    async function buscarLibro(isbn) {
        const res = await fetch('/api/Libros');
        if (!res.ok) throw new Error('Error al cargar libros');
        const libros = await res.json();
        return libros.find(l => (l.codigoIsbn === isbn || l.isbn === isbn));
    }

    if (formPrestamo) {
        formPrestamo.addEventListener('submit', async (e) => {
            e.preventDefault();

            const dni = document.getElementById('dni').value.trim();
            const isbn = document.getElementById('isbn').value.trim();
            const fechaHoraLimite = document.getElementById('datetime').value;
            const btnSubmit = formPrestamo.querySelector('button[type="submit"]');

            const infoDni = document.getElementById('info-dni');
            const infoIsbn = document.getElementById('info-isbn');
            if (infoDni) infoDni.classList.add('hidden');
            if (infoIsbn) infoIsbn.classList.add('hidden');

            // --- ANTI-CONGELAMIENTO (Deshabilitar Botón) ---
            const textoOriginal = btnSubmit.innerHTML;
            btnSubmit.disabled = true;
            btnSubmit.innerHTML = '<span class="material-symbols-outlined animate-spin font-bold text-3xl" style="animation: spin 1s linear infinite;">progress_activity</span> CARGANDO...';

            try {
                // Validación Estudiante
                const estudiante = await buscarEstudiante(dni);
                if (!estudiante) {
                    alert('Estudiante no encontrado con ese DNI.');
                    btnSubmit.disabled = false;
                    btnSubmit.innerHTML = textoOriginal;
                    return;
                }

                // --- REGLA DE NEGOCIO (MOROSO) ---
                if (estudiante.esMoroso) {
                    alert('Operación Bloqueada: El estudiante es MOROSO. No puede realizar nuevos préstamos hasta devolver los pendientes vencidos.');
                    btnSubmit.disabled = false;
                    btnSubmit.innerHTML = textoOriginal;
                    return;
                }

                // Validación Libro
                const libro = await buscarLibro(isbn);
                if (!libro) {
                    alert('Libro no encontrado con ese ISBN.');
                    btnSubmit.disabled = false;
                    btnSubmit.innerHTML = textoOriginal;
                    return;
                }

                const stockDisp = libro.stockDisponible !== undefined ? libro.stockDisponible : libro.stockTotal;
                if (stockDisp <= 0) {
                    alert('No hay stock disponible para este libro.');
                    btnSubmit.disabled = false;
                    btnSubmit.innerHTML = textoOriginal;
                    return;
                }

                // Payload
                const payload = {
                    usuarioId: userInfo.id,
                    estudianteId: estudiante.id,
                    libroId: libro.id,
                    fechaHoraLimite: new Date(fechaHoraLimite).toISOString()
                };

                const res = await fetch('/api/Prestamos', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(payload)
                });

                if (res.ok) {
                    alert('Préstamo registrado exitosamente.');
                    formPrestamo.reset();
                    cargarPrestamosPendientes();
                } else {
                    const err = await res.text();
                    alert('No se pudo registrar el préstamo: ' + err);
                }

            } catch (error) {
                console.error(error);
                alert('Ocurrió un error en la validación o conexión.');
            } finally {
                // --- ANTI-CONGELAMIENTO (Rehabilitar Botón) ---
                btnSubmit.disabled = false;
                btnSubmit.innerHTML = textoOriginal;
            }
        });
    }

    async function cargarPrestamosPendientes() {
        if (!contenedorPrestamos) return;
        
        try {
            const res = await fetch('/api/Prestamos');
            if (!res.ok) throw new Error('Error al cargar préstamos');
            let prestamos = await res.json();
            
            // Filtramos solo pendientes (estado === 0)
            prestamos = prestamos.filter(p => p.estado === 0);
            
            contenedorPrestamos.innerHTML = '';
            
            if (prestamos.length === 0) {
                contenedorPrestamos.innerHTML = '<p class="text-on-surface-variant font-bold">No hay préstamos pendientes.</p>';
                return;
            }
            
            prestamos.forEach(p => {
                const ahora = new Date();
                const limite = new Date(p.fechaHoraLimite);
                const vencido = ahora > limite;
                
                const card = document.createElement('div');
                card.className = `p-4 border-4 border-on-background neubrutalism-shadow ${vencido ? 'bg-error-container text-on-error-container' : 'bg-surface text-on-surface'}`;
                
                card.innerHTML = `
                    <div class="flex justify-between items-start mb-2">
                        <h3 class="font-headline-sm font-bold truncate pr-4 text-on-background">${p.libroTitulo}</h3>
                        <span class="px-2 py-1 font-label-bold text-xs uppercase border-2 border-on-background ${vencido ? 'bg-error text-white' : 'bg-primary text-white'}">
                            ${vencido ? 'VENCIDO' : 'A TIEMPO'}
                        </span>
                    </div>
                    <p class="font-body-md text-on-background"><strong>Estudiante:</strong> ${p.estudianteNombre}</p>
                    <p class="font-body-md text-on-background"><strong>Fecha Préstamo:</strong> ${new Date(p.fechaHoraPrestamo).toLocaleString()}</p>
                    <p class="font-body-md text-on-background mb-4"><strong>Límite:</strong> ${limite.toLocaleString()}</p>
                    <button class="btn-devolver w-full py-2 bg-inverse-primary text-on-primary-fixed border-2 border-on-background font-label-bold uppercase flex items-center justify-center gap-2 hover-press transition-all">
                        <span class="material-symbols-outlined font-bold">assignment_return</span>
                        REGISTRAR ENTRADA
                    </button>
                `;
                
                const btnDevolver = card.querySelector('.btn-devolver');
                btnDevolver.addEventListener('click', async () => {
                    if (confirm(`¿Confirmar la devolución del libro "${p.libroTitulo}"?`)) {
                        
                        // --- ANTI-CONGELAMIENTO ---
                        const originalText = btnDevolver.innerHTML;
                        btnDevolver.disabled = true;
                        btnDevolver.innerHTML = '<span class="material-symbols-outlined animate-spin font-bold" style="animation: spin 1s linear infinite;">progress_activity</span> CARGANDO...';
                        
                        try {
                            const devolRes = await fetch(`/api/Prestamos/${p.id}/devolucion`, {
                                method: 'PUT'
                            });
                            
                            if (devolRes.ok) {
                                alert('Devolución registrada correctamente.');
                                // Si el estudiante estaba moroso, deberíamos actualizar su estado si devuelve, pero eso lo hace el backend o la lógica de negocio en la API.
                                cargarPrestamosPendientes();
                            } else {
                                const errText = await devolRes.text();
                                alert('Error al registrar devolución: ' + errText);
                            }
                        } catch (err) {
                            console.error(err);
                            alert('Error de conexión al devolver.');
                        } finally {
                            btnDevolver.disabled = false;
                            btnDevolver.innerHTML = originalText;
                        }
                    }
                });
                
                contenedorPrestamos.appendChild(card);
            });
            
        } catch (error) {
            console.error(error);
            contenedorPrestamos.innerHTML = '<p class="text-error font-bold">Ocurrió un error al cargar los préstamos.</p>';
        }
    }

    if (contenedorPrestamos) {
        cargarPrestamosPendientes();
        setInterval(cargarPrestamosPendientes, 60000); // refresh cada minuto
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
                
                const headerUserName = document.getElementById('header-user-name');
                const headerUserRole = document.getElementById('header-user-role');
                const profileImg = document.getElementById('header-profile-img');

                if (headerUserName) headerUserName.textContent = nombres;
                if (headerUserRole) headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (pisoArea || 'Bibliotecario');

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
"""

target_path = r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\js\prestamos.js'
with open(target_path, 'w', encoding='utf-8') as f:
    f.write(js_code)
print("prestamos.js generated perfectly.")
