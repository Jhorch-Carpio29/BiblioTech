import os

files = [
    r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\admin_dashboard.html',
    r'd:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\admin_personal.html'
]

for p in files:
    if not os.path.exists(p): continue
    with open(p, 'r', encoding='utf-8') as f:
        c = f.read()

    # Settings
    c = c.replace(
        '<a class="flex items-center gap-3 py-2 text-on-surface-variant hover:text-primary transition-colors font-bold" href="#">\n<span class="material-symbols-outlined">settings</span> Settings',
        '<a id="btn-configuracion" class="cursor-pointer flex items-center gap-3 py-2 text-on-surface-variant hover:text-primary transition-colors font-bold">\n<span class="material-symbols-outlined">settings</span> Configuración'
    )

    # Logout
    c = c.replace(
        '<a class="flex items-center gap-3 py-2 text-on-surface-variant hover:text-error transition-colors font-bold" href="#">\n<span class="material-symbols-outlined">logout</span> Logout',
        '<a id="btn-logout" class="cursor-pointer flex items-center gap-3 py-2 text-on-surface-variant hover:text-error transition-colors font-bold">\n<span class="material-symbols-outlined">logout</span> Cerrar Sesión'
    )
    
    # Profile Name
    c = c.replace(
        '<p class="font-body-sm text-body-sm mt-2 font-bold"><span style="color: rgb(68, 73, 51); text-transform: uppercase; background-color: rgb(255, 215, 245);">UNSCH Admin</span></p>',
        '<p id="sidebar-user-role-text" class="font-body-sm text-body-sm mt-2 font-bold"><span style="color: rgb(68, 73, 51); text-transform: uppercase; background-color: rgb(255, 215, 245);">UNSCH Admin</span></p>'
    )
    
    # Image Header ID
    c = c.replace('<img alt="Admin UNSCH Profile Photo"', '<img id="header-profile-img" alt="Admin UNSCH Profile Photo"')

    with open(p, 'w', encoding='utf-8') as f:
        f.write(c)

print("Injected UI IDs perfectly")
