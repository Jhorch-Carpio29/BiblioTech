import os
import re

files_to_edit = [
    r"d:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\dashboard.html",
    r"d:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\estudiantes.html",
    r"d:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\inventario.html",
    r"d:\UNSCH\400 I\Pruebas y Aseguramiento de Calidad de Software\LaboratorioZapata\EXAMEN 01\SISTEMA DE BIBLIOTECA\BiblioTech\BiblioTech.API\wwwroot\prestamos.html"
]

policy_text = "Políticas de Préstamo:\\n\\n- Límite de tiempo de entrega estricto.\\n- Máximo de 3 libros prestados por estudiante."

for p in files_to_edit:
    if not os.path.exists(p):
        print(f"File not found: {p}")
        continue
    with open(p, 'r', encoding='utf-8') as f:
        c = f.read()

    if 'id="btn-politicas"' in c:
        print(f"Already injected in {p}")
        continue

    # Regex to find btn-configuracion and its closing </a> tag
    pattern = re.compile(r'(<a id="btn-configuracion"[^>]*>.*?</a>)', re.DOTALL)
    
    def replacer(match):
        config_html = match.group(1)
        
        # Extract class from config_html
        class_match = re.search(r'class="([^"]+)"', config_html)
        classes = class_match.group(1) if class_match else ''
        
        # Determine span based on config_html
        span_match = re.search(r'<span[^>]*>.*?</span>', config_html, re.DOTALL)
        if span_match:
            span_html = span_match.group(0)
            span_html = re.sub(r'>.*?</span>', '>policy</span>', span_html)
        else:
            span_html = '<span class="material-symbols-outlined">policy</span>'
            
        new_btn = f'\n<a id="btn-politicas" class="{classes} cursor-pointer" onclick="alert(\'{policy_text}\')">{span_html} Políticas de Préstamo</a>'
        return config_html + new_btn

    new_c = pattern.sub(replacer, c)
    
    with open(p, 'w', encoding='utf-8') as f:
        f.write(new_c)
    print(f"Injected in {p}")

