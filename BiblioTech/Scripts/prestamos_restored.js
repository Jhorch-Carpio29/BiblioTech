1: document.addEventListener('DOMContentLoaded', () => {
2:     // ============================================================
3:     // 1. CONTROL DE SESIÓN Y PERFIL DINÁMICO
4:     // ============================================================
5:     const userInfoStr = localStorage.getItem('usuarioInfo');
6:     if (!userInfoStr) {
7:         window.location.href = 'index.html';
8:         return;
9:     }
10: 
11:     const userInfo = JSON.parse(userInfoStr);
12: 
13:     // Rellenar header con datos del usuario logueado
14:     const headerUserName = document.getElementById('header-user-name');
15:     const headerUserRole = document.getElementById('header-user-role');
16:     const profileImg = document.getElementById('header-profile-img');
17: 
18:     if (headerUserName) headerUserName.textContent = userInfo.nombres || 'Admin';
19:     if (headerUserRole) headerUserRole.textContent = userInfo.rolId === 1 ? 'Superusuario' : (userInfo.pisoArea || 'Bibliotecario');
20:     if (profileImg) {
21:         profileImg.src = `https://ui-avatars.com/api/?name=${encodeURIComponent(userInfo.nombres || 'Admin')}&background=random&color=fff&bold=true`;
22:     }
23: 
24:     // Botón Logout
25:     const btnLogout = document.getElementById('btn-logout');
26:     if (btnLogout) {
27:         btnLogout.addEventListener('click', (e) => {
28:             e.preventDefault();
29:             localStorage.clear();
30:             window.location.href = 'index.
<truncated 21145 bytes>
==============
438:     // 10. UTILIDADES
439:     // ============================================================
440:     function mostrarInfo(elemento, mensaje, tipo) {
441:         if (!elemento) return;
442:         elemento.classList.remove('hidden', 'text-primary', 'text-error', 'text-on-surface-variant', 'bg-primary-container', 'bg-error-container', 'bg-surface-container-high');
443: 
444:         switch (tipo) {
445:             case 'success':
446:                 elemento.className = 'mt-1 px-3 py-2 font-body-sm text-body-sm border-2 border-on-background bg-primary-container text-on-primary-container';
447:                 break;
448:             case 'error':
449:                 elemento.className = 'mt-1 px-3 py-2 font-body-sm text-body-sm border-2 border-on-background bg-error-container text-on-error-container';
450:                 break;
451:             case 'warning':
452:                 elemento.className = 'mt-1 px-3 py-2 font-body-sm text-body-sm border-2 border-on-background bg-surface-container-high text-on-surface-variant';
453:                 break;
454:         }
455: 
456:         elemento.textContent = mensaje;
457:         elemento.classList.remove('hidden');
458:     }
459: 
460:     // ============================================================
461:     // 11. INICIALIZACIÓN
462:     // ============================================================
463:     // Establecer fecha mínima del input datetime-local como ahora
464:     if (inputDatetime) {
465:         const ahora = new Date();
466:         ahora.setMinutes(ahora.getMinutes() - ahora.getTimezoneOffset());
467:         inputDatetime.min = ahora.toISOString().slice(0, 16);
468:     }
469: 
470:     // Cargar préstamos pendientes al inicio
471:     cargarPrestamosPendientes();
472: 
473:     // Refrescar contadores de tiempo cada minuto
474:     setInterval(() => {
475:         cargarPrestamosPendientes();
476:     }, 60000);
477: });
478: 