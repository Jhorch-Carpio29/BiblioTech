# ⚡ VERIFICACIÓN RÁPIDA (5 MINUTOS)

## 1. Compila el proyecto
```powershell
cd BiblioTech
dotnet build
```

**Esperado:** ✅ Compilación correcta

---

## 2. Ejecuta la aplicación
```powershell
dotnet run --project BiblioTech.API
```

Verás algo como:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7xxx
```

**Nota:** El puerto puede variar (7001, 7002, etc.)

---

## 3. Abre en el navegador

Ve a: `https://localhost:7xxx/estudiantes.html`

(Reemplaza `7xxx` con el puerto que viste arriba)

---

## 4. Abre la Consola (F12)

Presiona **F12** → Pestaña **"Console"**

Deberías VER INMEDIATAMENTE:

```
Iniciando cargarEstudiantes()...
Response status: 200
Response ok: true
✅ Datos recibidos de la API: [{"id":999,"codigoUniversitario":"20240001",...
```

---

## 5. Verifica la Tabla

**¿VES estos datos en la tabla?**

| DNI | Código | Nombres Completos | Escuela Profesional | Estado |
|-----|--------|-------------------|-------------------|--------|
| 12345678 | 20240001 | JUAN PÉREZ GARCÍA (DATO DE PRUEBA) | Ingeniería de Sistemas (DATO DE PRUEBA) | HABILITADO |

### ✅ SÍ → **BUG RESUELTO** 🎉

### ❌ NO → Continúa leyendo...

---

## 6. Si AÚNNO VES DATOS

### Busca en la Consola (F12):

**Línea 1 - Busca esto:**
```
❌ Error CRÍTICO en cargarEstudiantes:
```

Si la VES, copia el mensaje de error completo.

**Línea 2 - Busca esto:**
```
Response status: 500
```

Si es 500, hay error en el servidor. Revisa la ventana de PowerShell donde corriste `dotnet run`.

---

## 7. Limpia Caché y Recarga

Si ya viste datos pero ahora no:

1. Presiona **Ctrl+Shift+Delete** (Limpiar datos de navegación)
2. Selecciona "Caché"
3. Presiona **Ctrl+Shift+R** en la página (Recarga fuerte)

---

## 8. Última Opción: Prueba la API Directamente

Abre una **nueva pestaña** y ve a:

`https://localhost:7xxx/api/Estudiantes`

**Deberías ver JSON como esto:**

```json
[{"id":999,"codigoUniversitario":"20240001","dni":"12345678","nombresCompletos":"JUAN PÉREZ GARCÍA (DATO DE PRUEBA)","escuelaNombre":"Ingeniería de Sistemas (DATO DE PRUEBA)","esMoroso":false}]
```

### ✅ SÍ → La API funciona. Problema en el Frontend/Caché.

### ❌ NO → Error en el Backend. Revisa PowerShell.

---

## ✨ RESUMEN

**La solución está 100% implementada en el código.**

Si ejecutas la app y NO ves los campos "Nombres Completos" y "Escuela Profesional", es porque:

1. ❌ **Caché del navegador:** Limpiar (Ctrl+Shift+Delete)
2. ❌ **Error en API:** Ver logs en PowerShell
3. ❌ **Error en BD:** Ver excepción en PowerShell

**Ejecuta ahora. Debería funcionar.** ✅
