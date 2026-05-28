document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const errorMessage = document.getElementById('error-message');

    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            errorMessage.classList.add('hidden');
            
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;

            try {
                const response = await fetch('/api/Usuarios/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email, password })
                });

                if (response.ok) {
                    const data = await response.json();

                    // Guardar en localStorage
                    localStorage.setItem('usuarioInfo', JSON.stringify(data));

                    // Redirigir dependiendo del rol
                    if (data.rolId === 1) { // 1 = Admin
                        window.location.href = 'admin_dashboard.html';
                    } else { // 2 = Bibliotecario...
                        window.location.href = 'dashboard.html';
                    }
                } else if (response.status === 401) {
                    const errData = await response.json();
                    errorMessage.textContent = errData.message || 'Credenciales incorrectas';
                    errorMessage.classList.remove('hidden');
                } else {
                    errorMessage.textContent = 'Error al conectarse con el servidor';
                    errorMessage.classList.remove('hidden');
                }
            } catch (err) {
                console.error(err);
                errorMessage.textContent = 'Error de red. Intenta nuevamente.';
                errorMessage.classList.remove('hidden');
            }
        });
    }
});
