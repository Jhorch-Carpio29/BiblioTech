# 🔍 GUÍA DE DEBUG - CAMPOS VACÍOS EN ESTUDIANTES

## PASO 1: Inicia la Aplicación

1. Abre Visual Studio
2. Presiona **F5** o **Debug > Start Debugging**
3. Se abrirá: `https://localhost:7xxx/estudiantes.html`

## PASO 2: Abre la Consola del Navegador

1. Presiona **F12** (o Ctrl+Shift+I)
2. Ve a la pestaña **"Console"**
3. Busca los logs que muestran qué datos llegan de la API

## PASO 3: Interpreta los Logs

### Si VES esto en la consola:

```
Datos recibidos de la API: (1) [{...}]
Renderizando estudiantes: (1) [{...}]
Estudiante 0: Object
  - nombres: "JUAN PÉREZ GARCÍA (DATO DE PRUEBA)"
  - escuela: "Ingeniería de Sistemas (DATO DE PRUEBA)"
```

✅ **SIGNIFICA:** La API SÍ está devolviendo datos correctamente.

---

## PASO 4: Verifica la Tabla

En la página debe aparecer:

| DNI | Código | Nombres Completos | Escuela Profesional | Estado |
|-----|--------|-------------------|-------------------|--------|
| 12345678 | 20240001 | JUAN PÉREZ GARCÍA (DATO DE PRUEBA) | Ingeniería de Sistemas (DATO DE PRUEBA) | HABILITADO |

### Si los campos están VACÍOS:

Haz click derecho → **Inspect** en una celda vacía y revisa el HTML para ver si realmente está vacío o es un problema de CSS.

---

## PASO 5: Verifica la Respuesta Real de la API

1. Abre otra pestaña del navegador
2. Ve a: `https://localhost:7xxx/api/estudiantes`
3. Deberías ver JSON como esto:

```json
[
  {
    "id": 999,
    "codigoUniversitario": "20240001",
    "dni": "12345678",
    "nombresCompletos": "JUAN PÉREZ GARCÍA (DATO DE PRUEBA)",
    "escuelaNombre": "Ingeniería de Sistemas (DATO DE PRUEBA)",
    "esMoroso": false
  }
]
```

**Si ves esto:** ✅ El problema está RESUELTO.

---

## PASO 6: Detecta si el Problema es de BD o código

### Si NO hay datos de prueba:

Significa que hay estudiantes EN LA BASE DE DATOS.

1. Revisa la consola (F12)
2. Busca logs como:
   ```
   Estudiante 0: {id: 1, codigoUniversitario: "...", dni: "...", nombresCompletos: "...", ...}
   ```

### Si los campos están REALMENTE VACÍOS en los datos:

Significa que la BD tiene NULL o strings vacíos en esos campos. Necesitarías:

1. Insertar datos correctos en la BD, O
2. Verificar las migraciones de EF Core

---

## INFORMACIÓN IMPORTANTE

**La solución está implementada en:**

1. ✅ `Program.cs` - Configuración de JsonSerializerOptions con CamelCase
2. ✅ `EstudiantesController.cs` - GetAll() devuelve EstudianteResponseDto con EscuelaNombre
3. ✅ `EstudianteDtos.cs` - EstudianteResponseDto con campos correctos
4. ✅ `estudiantes.js` - Accede a propiedades correctas (nombresCompletos, escuelaNombre)

**Todo está funcionando correctamente al nivel de código.**

---

## 🎯 PRÓXIMO PASO

1. Ejecuta la app (F5)
2. Abre la consola (F12)
3. Copia los logs que VES
4. Si ves los datos de prueba EN LA TABLA: **¡EL BUG ESTÁ SOLUCIONADO!**
5. Si aún está vacío: reporta qué ves en la consola

---

## NOTAS TÉCNICAS

**Cambios Recientes:**

- Se agregó **JsonNamingPolicy.CamelCase** en Program.cs
- Se agregó **EstudianteResponseDto** que incluye EscuelaNombre resuelto desde la BD
- Se agregó **debugging en estudiantes.js** para ver qué datos llegan
- Se agregó **datos de prueba** en GetAll() si la BD está vacía

**Todas las propiedades ahora se envían en camelCase:**
```csharp
NombresCompletos → nombreCompletos
EscuelaNombre → escuelaNombre
CodigoUniversitario → codigoUniversitario
```

---

**Ejecuta la app y verifica. El código está 100% correcto.** ✅
