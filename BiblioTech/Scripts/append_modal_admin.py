import sys
import re

modal_html = """
<!-- Modal Mi Perfil -->
<div id="modal-perfil" class="hidden fixed inset-0 z-[70] flex justify-end bg-black bg-opacity-50">
    <div class="w-[450px] bg-surface h-full flex flex-col shadow-[inset_4px_0_0_0_#1c1b1b]">
        <div class="p-6 border-b-border-width border-on-surface flex justify-between items-center bg-surface-container-low">
            <h2 class="font-headline-sm text-headline-sm font-bold uppercase flex items-center gap-2">
                <span class="material-symbols-outlined">manage_accounts</span> Mi Perfil
            </h2>
            <button id="btn-cerrar-perfil" class="text-on-surface hover:text-error transition-colors">
                <span class="material-symbols-outlined text-[28px]">close</span>
            </button>
        </div>
        <div class="p-6 flex-1 overflow-y-auto">
            <div id="perfil-loading" class="flex items-center justify-center py-10">
                <span class="material-symbols-outlined animate-spin text-4xl">progress_activity</span>
            </div>
            <form id="form-perfil" class="hidden flex flex-col gap-5">
                <div class="flex flex-col gap-1 opacity-70">
                    <label class="font-label-bold text-label-bold uppercase">DNI (No editable)</label>
                    <input type="text" id="perfil-dni" class="border-border-width border-on-surface bg-surface-variant p-3 font-body-md" readonly>
                </div>
                <div class="flex flex-col gap-1">
                    <label for="perfil-nombres" class="font-label-bold text-label-bold uppercase">Nombres y Apellidos</label>
                    <input type="text" id="perfil-nombres" class="border-border-width border-on-surface bg-background p-3 shadow-[4px_4px_0px_0px_rgba(28,27,27,1)] font-body-md focus:bg-primary-container focus:outline-none transition-colors" required>
                </div>
                <div class="flex flex-col gap-1">
                    <label for="perfil-email" class="font-label-bold text-label-bold uppercase">Correo Institucional</label>
                    <input type="email" id="perfil-email" class="border-border-width border-on-surface bg-background p-3 shadow-[4px_4px_0px_0px_rgba(28,27,27,1)] font-body-md focus:bg-primary-container focus:outline-none transition-colors" required>
                </div>
                <div class="flex flex-col gap-1">
                    <label for="perfil-area" class="font-label-bold text-label-bold uppercase">Piso / Área Asignada</label>
                    <input type="text" id="perfil-area" class="border-border-width border-on-surface bg-background p-3 shadow-[4px_4px_0px_0px_rgba(28,27,27,1)] font-body-md focus:bg-primary-container focus:outline-none transition-colors" required>
                </div>
                <div class="bg-surface-container-highest border-l-4 border-primary p-4 mt-2">
                    <p class="font-body-sm text-body-sm text-on-surface-variant flex items-start gap-2">
                        <span class="material-symbols-outlined text-base mt-0.5">info</span>
                        Tus cambios se reflejarán en todo el sistema.
                    </p>
                </div>
            </form>
        </div>
        <div class="p-6 border-t-border-width border-on-surface bg-surface-container-low">
            <button id="btn-guardar-perfil" form="form-perfil" type="submit" class="hidden w-full bg-primary-fixed text-on-primary-fixed border-border-width border-on-surface py-4 px-6 font-label-bold text-label-bold uppercase shadow-[8px_8px_0px_0px_rgba(28,27,27,1)] hover:translate-x-1 hover:translate-y-1 hover:shadow-[4px_4px_0px_0px_rgba(28,27,27,1)] active:translate-x-2 active:translate-y-2 active:shadow-none transition-all text-center flex items-center justify-center gap-2">
                <span class="material-symbols-outlined">save</span>
                Guardar Cambios
            </button>
        </div>
    </div>
</div>
"""

def append_modal(filepath):
    print(f"Processing {filepath}")
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    if "modal-perfil" in content:
        print(f"Modal already exists in {filepath}")
        return
        
    # Append before </body>
    idx = content.rfind("</body>")
    if idx == -1:
        # Just append at end
        content += "\n" + modal_html
    else:
        content = content[:idx] + "\n" + modal_html + "\n" + content[idx:]
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)
        
    print(f"Modal appended to {filepath}")

for f in ['admin_personal.html']:
    append_modal('d:/UNSCH/400 I/Pruebas y Aseguramiento de Calidad de Software/LaboratorioZapata/EXAMEN 01/SISTEMA DE BIBLIOTECA/BiblioTech/BiblioTech.API/wwwroot/' + f)

