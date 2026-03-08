# 📦 Instalación de Swagger (Swashbuckle) - Guía Paso a Paso

## 🚀 Instalación Paso a Paso en Visual Studio

### **Paso 1: Instalar Swashbuckle via NuGet**

1. **Clic derecho** en el proyecto `Personas.API` en el Explorador de soluciones
2. Selecciona: **"Administrar paquetes NuGet..."**
3. Ve a la pestaña **"Examinar"**
4. En el cuadro de búsqueda, escribe: **`Swashbuckle`**
5. Selecciona **"Swashbuckle"** (by Richard Morris)
6. En el panel derecho, selecciona versión: **5.6.0**
7. Haz clic en **"Instalar"**
8. Acepta las licencias si aparece el cuadro de diálogo

**✅ Esto instalará automáticamente:**
- Swashbuckle.Core
- WebActivatorEx
- Y creará el archivo `SwaggerConfig.cs` en `App_Start`

---

### **Paso 2: Habilitar Documentación XML**

1. **Clic derecho** en el proyecto `Personas.API` → **Propiedades**
2. Ve a la pestaña **"Compilar"** (Build)
3. Baja hasta la sección **"Salida"** (Output)
4. ✅ Marca la casilla: **"Archivo de documentación XML"**
5. En el campo, debería aparecer: `bin\Personas.API.xml`
6. **Guarda** (Ctrl+S) y cierra la ventana de propiedades

---

### **Paso 3: Suprimir Advertencias XML (Opcional)**

Para evitar advertencias de "Missing XML comment":

1. En la misma pestaña **"Compilar"**
2. Busca el campo **"Suprimir advertencias"** (Suppress warnings)
3. Agrega: `1591`
4. **Guarda** (Ctrl+S)

---

### **Paso 4: Compilar el Proyecto**

1. Presiona **Ctrl+Shift+B** o clic en **Compilar → Compilar solución**
2. Verifica que no haya errores en la ventana de salida

---

### **Paso 5: Ejecutar el Proyecto**

1. Presiona **F5** para ejecutar
2. Navega a: **`http://localhost:11947/swagger`**

---

## 🎯 Lo que Verás en Swagger UI

```
╔════════════════════════════════════════════════════════╗
║  API de Gestión de Personas v1                         ║
║  Microservicio para la gestión de personas             ║
╠════════════════════════════════════════════════════════╣
║  Personas                                              ║
╠════════════════════════════════════════════════════════╣
║  ▼ GET    /api/personas                                ║
║     Obtiene todas las personas (médicos y pacientes)   ║
║                                                        ║
║  ▼ POST   /api/personas                                ║
║     Crea una nueva persona (médico o paciente)         ║
║                                                        ║
║  ▼ GET    /api/personas/{id}                           ║
║     Obtiene una persona por su ID                      ║
║                                                        ║
║  ▼ PUT    /api/personas/{id}                           ║
║     Actualiza una persona existente                    ║
║                                                        ║
║  ▼ DELETE /api/personas/{id}                           ║
║     Elimina una persona                                ║
║                                                        ║
║  ▼ GET    /api/personas/medicos                        ║
║     Obtiene todos los médicos                          ║
║                                                        ║
║  ▼ GET    /api/personas/pacientes                      ║
║     Obtiene todos los pacientes                        ║
╠════════════════════════════════════════════════════════╣
║  Models                                                ║
╠════════════════════════════════════════════════════════╣
║  ▼ PersonaDto                                          ║
║     {                                                  ║
║       "id": 0,                                         ║
║       "nombre": "string",                              ║
║       "apellido": "string",                            ║
║       "numeroDocumento": "string",                     ║
║       "tipoDocumento": "Cedula",                       ║
║       "tipoPersona": "Paciente",                       ║
║       "fechaNacimiento": "2024-03-07T14:54:59.000Z"    ║
║     }                                                  ║
║                                                        ║
║  ▼ TipoDocumento                                       ║
║     - Cedula = 1                                       ║
║     - Pasaporte = 2                                    ║
║     - TarjetaIdentidad = 3                             ║
║                                                        ║
║  ▼ TipoPersona                                         ║
║     - Paciente = 1                                     ║
║     - Medico = 2                                       ║
╚════════════════════════════════════════════════════════╝
```

---

## 🧪 Probar Endpoints desde Swagger

### **Ejemplo: POST /api/personas**

1. Haz clic en **POST /api/personas**
2. Clic en **"Try it out"**
3. Edita el JSON de ejemplo:

```json
{
  "id": 0,
  "nombre": "Dr. Roberto",
  "apellido": "Sánchez",
  "numeroDocumento": "M001234567",
  "tipoDocumento": 1,
  "tipoPersona": 2,
  "fechaNacimiento": "1980-05-15T00:00:00"
}
```

4. Clic en **"Execute"**
5. Verás la respuesta:

```json
{
  "mensaje": "Persona creada exitosamente"
}
```

---

## ⚙️ Configuración Avanzada (Opcional)

### **Personalizar la Información de la API**

Edita `SwaggerConfig.cs`:

```csharp
c.SingleApiVersion("v1", "API de Gestión de Personas")
    .Description("Tu descripción personalizada")
    .Contact(cc => cc
        .Name("Tu Nombre")
        .Email("tu@email.com"));
```

### **Agregar Autenticación JWT (Futuro)**

```csharp
c.ApiKey("Bearer")
    .Description("JWT Authorization header using the Bearer scheme")
    .Name("Authorization")
    .In("header");
```

---

## 📋 Estructura de Archivos Swagger

```
Personas.API/
├── App_Start/
│   └── SwaggerConfig.cs          ← Configuración de Swagger
├── bin/
│   └── Personas.API.xml          ← Documentación XML (se genera)
└── packages.config               ← Swashbuckle agregado
```

---

## ✅ Checklist de Instalación

- [ ] `packages.config` tiene Swashbuckle 5.6.0
- [ ] Restaurar paquetes NuGet completado
- [ ] Documentación XML habilitada en Propiedades del proyecto
- [ ] SwaggerConfig.cs existe en App_Start
- [ ] Proyecto compila sin errores
- [ ] Swagger UI accesible en /swagger

---

## 🐛 Solución de Problemas

### Error: "Could not load file or assembly 'Swashbuckle.Core'"

**Solución:**
```powershell
# En la Consola del Administrador de Paquetes
Install-Package Swashbuckle -Version 5.6.0 -ProjectName Personas.API
```

### Error: Swagger UI muestra página en blanco

**Solución:**
1. Verifica que estés navegando a `/swagger` (no `/swagger/ui`)
2. Limpia y reconstruye la solución (Ctrl+Shift+B)
3. Verifica que no haya errores de compilación

### Error: "No XML comments found"

**Solución:**
1. Verifica que la documentación XML esté habilitada
2. Verifica que el archivo `Personas.API.xml` se esté generando en `bin/`
3. Recompila el proyecto

---

## 🎉 ¡Listo!

Una vez completados estos pasos, tendrás:

✅ Swagger UI funcionando en `/swagger`  
✅ Documentación automática de todos los endpoints  
✅ Capacidad de probar la API directamente desde el navegador  
✅ Esquemas de DTOs y Enums documentados  
✅ Arquitectura DDD intacta (Swagger solo en API layer)  

---

## 📱 URLs Útiles

- **Swagger UI**: `http://localhost:11947/swagger`
- **Swagger JSON**: `http://localhost:11947/swagger/docs/v1`
- **API Base**: `http://localhost:11947/api/personas`

---

**¡Disfruta de tu API documentada!** 🚀
