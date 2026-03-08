# 🧪 Guía de Prueba - Endpoint POST Personas

## 🚀 Paso 1: Ejecutar el Proyecto

1. En Visual Studio, presiona **F5** o haz clic en el botón ▶️ **Iniciar**
2. El navegador se abrirá automáticamente
3. Anota el **puerto** que se muestra en la URL (ej: `http://localhost:12345`)

---

## 📋 Paso 2: Preparar el Request

### **Endpoint**
```
POST http://localhost:[PUERTO]/api/personas
```

### **Headers**
```
Content-Type: application/json
```

### **Body (JSON)**

#### 🩺 **Crear un Médico**
```json
{
  "nombre": "Roberto",
  "apellido": "Sánchez",
  "numeroDocumento": "M001234567",
  "tipoDocumento": 1,
  "tipoPersona": 2,
  "fechaNacimiento": "1980-05-15T00:00:00"
}
```

#### 🏥 **Crear un Paciente**
```json
{
  "nombre": "María",
  "apellido": "González",
  "numeroDocumento": "1234567890",
  "tipoDocumento": 1,
  "tipoPersona": 1,
  "fechaNacimiento": "1995-03-20T00:00:00"
}
```

---

## 🛠️ Opciones para Probar el Endpoint

### **Opción 1: Postman** ⭐ (Recomendado)

1. Descarga [Postman](https://www.postman.com/downloads/)
2. Crea un nuevo request:
   - **Método**: POST
   - **URL**: `http://localhost:[PUERTO]/api/personas`
   - **Headers**: `Content-Type: application/json`
   - **Body** → **raw** → **JSON**: Pega uno de los JSON de arriba
3. Haz clic en **Send**

**Respuesta esperada (200 OK):**
```json
{
  "mensaje": "Persona creada exitosamente"
}
```

---

### **Opción 2: Thunder Client** (Extensión de VS Code)

Si usas VS Code:
1. Instala la extensión **Thunder Client**
2. New Request → POST
3. URL: `http://localhost:[PUERTO]/api/personas`
4. Body → JSON → Pega el JSON
5. Send

---

### **Opción 3: PowerShell / CMD**

```powershell
# Médico
Invoke-RestMethod -Uri "http://localhost:12345/api/personas" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{
    "nombre": "Roberto",
    "apellido": "Sánchez",
    "numeroDocumento": "M001234567",
    "tipoDocumento": 1,
    "tipoPersona": 2,
    "fechaNacimiento": "1980-05-15T00:00:00"
  }'
```

```powershell
# Paciente
Invoke-RestMethod -Uri "http://localhost:12345/api/personas" `
  -Method Post `
  -ContentType "application/json" `
  -Body '{
    "nombre": "María",
    "apellido": "González",
    "numeroDocumento": "1234567890",
    "tipoDocumento": 1,
    "tipoPersona": 1,
    "fechaNacimiento": "1995-03-20T00:00:00"
  }'
```

---

### **Opción 4: cURL** (Git Bash / Linux / Mac)

```bash
# Médico
curl -X POST http://localhost:12345/api/personas \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Roberto",
    "apellido": "Sánchez",
    "numeroDocumento": "M001234567",
    "tipoDocumento": 1,
    "tipoPersona": 2,
    "fechaNacimiento": "1980-05-15T00:00:00"
  }'
```

---

## 🎯 Validación de Campos

### **TipoDocumento (Enum)**
- `1` = Cédula
- `2` = Pasaporte
- `3` = Tarjeta de Identidad

### **TipoPersona (Enum)**
- `1` = Paciente
- `2` = Médico

### **Campos Obligatorios**
- ✅ `nombre` (string, máx 100 caracteres)
- ✅ `apellido` (string, máx 100 caracteres)
- ✅ `numeroDocumento` (string, máx 50 caracteres)
- ✅ `tipoDocumento` (int: 1, 2 o 3)
- ✅ `tipoPersona` (int: 1 o 2)
- ✅ `fechaNacimiento` (datetime)

---

## ✅ Verificar que se Creó

### **Opción A: SQL Server Management Studio**

```sql
USE PersonasDb;
SELECT * FROM Personas;
```

### **Opción B: Endpoint GET**

```
GET http://localhost:[PUERTO]/api/personas
```

Deberías ver tu registro creado.

---

## 🧪 Casos de Prueba

### ✅ **Caso Exitoso**
```json
POST /api/personas
Body: { "nombre": "Juan", ... }
Respuesta: 200 OK - "Persona creada exitosamente"
```

### ❌ **Caso de Error: Nombre vacío**
```json
POST /api/personas
Body: { "nombre": "", ... }
Respuesta: 400 Bad Request - Error de validación
```

### ❌ **Caso de Error: Tipo de documento inválido**
```json
POST /api/personas
Body: { "tipoDocumento": 99, ... }
Respuesta: Error - Enum inválido
```

---

## 📊 Datos de Prueba Completos

```json
[
  {
    "nombre": "Dr. Roberto",
    "apellido": "Sánchez",
    "numeroDocumento": "M001234567",
    "tipoDocumento": 1,
    "tipoPersona": 2,
    "fechaNacimiento": "1980-05-15T00:00:00"
  },
  {
    "nombre": "Dra. Laura",
    "apellido": "Fernández",
    "numeroDocumento": "M009876543",
    "tipoDocumento": 1,
    "tipoPersona": 2,
    "fechaNacimiento": "1985-08-22T00:00:00"
  },
  {
    "nombre": "María",
    "apellido": "González",
    "numeroDocumento": "1234567890",
    "tipoDocumento": 1,
    "tipoPersona": 1,
    "fechaNacimiento": "1995-03-20T00:00:00"
  },
  {
    "nombre": "Carlos",
    "apellido": "Rodríguez",
    "numeroDocumento": "AB123456",
    "tipoDocumento": 2,
    "tipoPersona": 1,
    "fechaNacimiento": "1990-11-10T00:00:00"
  }
]
```

---

## 🔍 Troubleshooting

### Error: "Connection refused" o "No se puede conectar"
- ✅ Verifica que el proyecto esté corriendo (F5)
- ✅ Verifica el puerto correcto en la URL

### Error: "404 Not Found"
- ✅ Verifica la URL: `http://localhost:[PUERTO]/api/personas`
- ✅ No debe tener `/` al final

### Error: "500 Internal Server Error"
- ✅ Revisa el Output de Visual Studio para ver el error exacto
- ✅ Verifica que la base de datos esté corriendo
- ✅ Verifica la cadena de conexión

### Error: "400 Bad Request"
- ✅ Verifica que el JSON esté bien formado
- ✅ Todos los campos obligatorios deben estar presentes
- ✅ Los tipos de datos deben ser correctos (int, string, datetime)

---

## 🎉 ¡Listo para Probar!

1. Ejecuta el proyecto (F5)
2. Abre Postman o tu herramienta favorita
3. Haz el POST con uno de los JSON de ejemplo
4. ¡Deberías ver el mensaje de éxito!

---

**💡 Tip**: Después de crear algunos registros, prueba los otros endpoints:
- `GET /api/personas` - Ver todos
- `GET /api/personas/{id}` - Ver uno específico
- `GET /api/personas/medicos` - Solo médicos
- `GET /api/personas/pacientes` - Solo pacientes
- `PUT /api/personas/{id}` - Actualizar
- `DELETE /api/personas/{id}` - Eliminar
