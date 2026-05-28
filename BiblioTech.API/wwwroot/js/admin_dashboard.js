document.addEventListener('DOMContentLoaded', async () => {
    // Control de sesión
    const userInfoStr = localStorage.getItem('usuarioInfo');
    if (!userInfoStr) {
        window.location.href = 'index.html';
        return;
    }

    const userInfo = JSON.parse(userInfoStr);
    
    // Perfil Header
    const headerUserName = document.getElementById('header-user-name');
    const headerUserRole = document.getElementById('header-user-role');
    const profileImg = document.querySelector('img[alt="Admin UNSCH Profile Photo"]');
    
    if (headerUserName) {
        headerUserName.textContent = userInfo.nombres;
        
        // Lógica simple para el avatar
        if (profileImg && userInfo.nombres) {
            profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres)}&background=random&color=fff&bold=true`;
        }
    }
    
    if (headerUserRole) {
        headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : 'Admin';
    }

    // Fecha actual
    const dateEl = document.getElementById('admin-current-date');
    if (dateEl) {
        const options = { weekday: 'long', day: 'numeric', month: 'short' };
        let dateStr = new Intl.DateTimeFormat('es-ES', options).format(new Date());
        // Capitalizar la primera letra
        dateStr = dateStr.charAt(0).toUpperCase() + dateStr.slice(1);
        dateEl.textContent = dateStr;
    }

    // Contadores y Tabla
    const countUsuarios = document.getElementById('admin-count-usuarios');
    const countLibros = document.getElementById('admin-count-libros');
    const countActivos = document.getElementById('admin-count-activos');
    const countMorosos = document.getElementById('admin-count-morosos');
    const tableMovimientos = document.getElementById('admin-table-movimientos');

    let todosLosPrestamos = []; // Almacena para exportación

    try {
        // Usuarios
        const resUsuarios = await fetch('/api/Usuarios');
        if (resUsuarios.ok) {
            const data = await resUsuarios.json();
            if (countUsuarios) countUsuarios.textContent = data.length;
        }

        // Libros
        const resLibros = await fetch('/api/Libros');
        if (resLibros.ok) {
            const data = await resLibros.json();
            if (countLibros) countLibros.textContent = data.length;
        }

        // Estudiantes (Morosos)
        const resEstudiantes = await fetch('/api/Estudiantes');
        if (resEstudiantes.ok) {
            const data = await resEstudiantes.json();
            const morosos = data.filter(e => e.esMoroso).length;
            if (countMorosos) countMorosos.textContent = morosos;
        }

        // Préstamos y Tabla Recientes
        const resPrestamos = await fetch('/api/Prestamos');
        if (resPrestamos.ok) {
            const data = await resPrestamos.json();
            todosLosPrestamos = data;

            const activos = data.filter(p => p.estado === 'Activo' || p.estado === 'Vigente').length;
            if (countActivos) countActivos.textContent = activos;

            // Actualizar tabla
            if (tableMovimientos) {
                tableMovimientos.innerHTML = '';
                const recientes = data.slice(-5).reverse();
                if (recientes.length === 0) {
                    tableMovimientos.innerHTML = '<tr><td colspan="3" class="p-4 text-center">No hay préstamos recientes</td></tr>';
                } else {
                    recientes.forEach(p => {
                        const tr = document.createElement('tr');
                        tr.className = 'border-b-4 border-on-background bg-surface-bright hover:bg-surface-variant transition-colors neubrutalist-border border-t-0';

                        let badgeClass = p.estado === 'Activo' ? 'bg-primary-container text-on-primary-fixed' 
                                    : p.estado === 'Devuelto' ? 'bg-tertiary-fixed text-on-tertiary-fixed'
                                    : 'bg-error-container text-on-error-container';

                        tr.innerHTML = `
                            <td class="p-4 font-body-lg text-body-lg font-bold border-r-4 border-on-background">${p.estudianteNombre || 'Desconocido'}</td>
                            <td class="p-4 font-body-lg text-body-lg border-r-4 border-on-background">${p.libroTitulo || 'Desconocido'}</td>
                            <td class="p-4">
                                <span class="inline-block px-3 py-1 ${badgeClass} font-label-bold text-label-bold uppercase neubrutalist-border shadow-[2px_2px_0px_0px_rgba(28,27,27,1)]">${p.estado}</span>
                            </td>
                        `;
                        tableMovimientos.appendChild(tr);
                    });
                }
            }

            // Actualizar Gráfico "Tráfico Semanal"
            const chartContainer = document.getElementById('traffic-chart');
            if (chartContainer) {
                const arrCounts = [0, 0, 0, 0, 0, 0, 0]; // Lunes a Domingo
                data.forEach(p => {
                    if(p.fechaHoraPrestamo) {
                        const d = new Date(p.fechaHoraPrestamo);
                        const idx = (d.getDay() + 6) % 7; // Convertir: Sunday=0 -> 6, Monday=1 -> 0
                        arrCounts[idx]++;
                    }
                });

                const maxVal = Math.max(...arrCounts, 1); // Evitar division por cero
                const bars = chartContainer.querySelectorAll('.bar');
                const labels = chartContainer.querySelectorAll('span.font-label-bold');

                bars.forEach((bar, index) => {
                    const count = arrCounts[index];
                    const heightPercent = (count / maxVal) * 100;
                    bar.style.height = `${heightPercent}%`;

                    if (labels[index]) {
                        // Limpiar texto anterior y añadir el número más el día
                        const dayText = labels[index].textContent.replace(/[0-9]/g, '').trim(); 
                        labels[index].innerHTML = `<div class="bg-surface border-2 border-on-background px-1 mb-1 shadow-[2px_2px_0px_0px_rgba(28,27,27,1)]">${count}</div>${dayText}`;
                    }
                });
            }
        }
    } catch (e) {
        console.error("Error cargando el dashboard del admin:", e);
    }

    // Exportar CSV
    const btnExportar = document.getElementById('btn-exportar');
    if (btnExportar) {
        btnExportar.addEventListener('click', () => {
            if (!todosLosPrestamos || todosLosPrestamos.length === 0) {
                alert('No hay datos para exportar en este momento.');
                return;
            }

            const headers = ["ID", "LibroTitulo", "EstudianteNombre", "Estado", "FechaHoraPrestamo"];
            let csvContent = headers.join(",") + "\n";

            todosLosPrestamos.forEach(p => {
                const row = [
                    p.id,
                    p.libroTitulo ? `"${p.libroTitulo.replace(/"/g, '""')}"` : "",
                    p.estudianteNombre ? `"${p.estudianteNombre.replace(/"/g, '""')}"` : "",
                    p.estado,
                    p.fechaHoraPrestamo
                ];
                csvContent += row.join(",") + "\n";
            });

            const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.setAttribute("href", url);
            link.setAttribute("download", "Reporte_BiblioTech_Trafico.csv");
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
    }
});