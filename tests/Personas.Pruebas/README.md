# Personas.Pruebas

Proyecto de **pruebas unitarias e integración** para el microservicio Personas, siguiendo principios de **Domain-Driven Design (DDD)** y **Test-Driven Development (TDD)**.

## 📋 Objetivo

Alcanzar **mínimo 35% de cobertura de código** (requisito académico) mediante pruebas que validen:

- ✅ **Lógica de negocio** (Domain)
- ✅ **Orquestación de casos de uso** (Application)
- ✅ **Validaciones y respuestas HTTP** (API)

## 🛠️ Herramientas Utilizadas

| Herramienta | Versión | Propósito |
|-------------|---------|-----------|
| **NUnit** | 3.13.3 | Framework de testing |
| **NUnit3TestAdapter** | 4.5.0 | Integración con Visual Studio |
| **Moq** | 4.18.4 | Mocking de dependencias |
| **FluentAssertions** | 6.12.0 | Aserciones expresivas |
| **OpenCover** | 4.7.1221 | Medición de cobertura |
| **ReportGenerator** | 5.1.26 | Reportes HTML de cobertura |

## 📁 Estructura del Proyecto

```
Personas.Pruebas/
│
├── Dominio/
│   ├── EntidadesPruebas/
│   │   └── PersonaPruebas.cs         ← Tests de entidad Persona
│   └── ValueObjectsPruebas/
│       └── DocumentoPruebas.cs        ← Tests de Value Object Documento
│
├── Aplicacion/
│   └── ServiciosPruebas/
│       └── PersonaServicePruebas.cs   ← Tests de servicio de aplicación
│
├── API/
│   └── ControllersPruebas/
│       └── PersonasControllerPruebas.cs ← Tests de controller
│
└── Helpers/
    └── TestDataBuilder.cs             ← Builders para datos de prueba
```

## ▶️ Ejecutar Tests

### Desde Visual Studio

1. **Test Explorer:**
   ```
   Test → Test Explorer → Run All
   ```

2. **Con cobertura (Visual Studio Enterprise):**
   ```
   Test → Analyze Code Coverage → All Tests
   ```

### Desde Línea de Comandos

**Ejecutar todos los tests:**
```powershell
.\packages\NUnit.ConsoleRunner.3.16.3\tools\nunit3-console.exe .\Personas.Pruebas\bin\Debug\Personas.Pruebas.dll
```

**Ejecutar tests con cobertura:**
```powershell
.\RunCoverage.bat
```

Esto genera:
- `coverage.xml` - Datos de cobertura
- `CoverageReport/index.html` - Reporte visual

## 📊 Objetivos de Cobertura

| Capa | Objetivo | Prioridad |
|------|----------|-----------|
| **Domain** | 90-100% | ⭐⭐⭐ CRÍTICA |
| **Application** | 80-90% | ⭐⭐ ALTA |
| **API** | 70-80% | ⭐⭐ ALTA |
| **Infrastructure** | 40-60% | ⭐ MEDIA (opcional) |
| **TOTAL** | **≥ 35%** | **REQUISITO MÍNIMO** |

## 🎯 Naming Conventions (Lenguaje Ubicuo)

Formato:
```
[ClaseTesteada]_[Escenario]_[ComportamientoEsperado]
```

Ejemplos:
```csharp
Persona_ConNombreVacio_DeberiaLanzarExcepcion()
PersonaService_CrearMedico_DeberiaLlamarRepositorio()
PersonasController_ObtenerPorId_DeberiaRetornar200OK()
```

## 📝 Patrón AAA (Arrange-Act-Assert)

Todos los tests siguen la estructura:

```csharp
[Test]
public void Persona_ConDocumentoValido_DeberiaCrearse()
{
    // ─── ARRANGE ───────────────────────────────────────
    var documento = new Documento(TipoDocumento.Cedula, "12345678");
    
    // ─── ACT ───────────────────────────────────────────
    var persona = new Persona("Juan", "Pérez", documento, TipoPersona.Medico);
    
    // ─── ASSERT ────────────────────────────────────────
    persona.Nombre.Should().Be("Juan");
    persona.Documento.Numero.Should().Be("12345678");
}
```

## 🔧 Instalación de Paquetes

Si necesitas reinstalar los paquetes:

```powershell
# En Package Manager Console
Update-Package -reinstall -ProjectName Personas.Pruebas
```

## 📚 Referencias

- [NUnit Documentation](https://docs.nunit.org/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions](https://fluentassertions.com/)
- [GUIA_TESTING_DDD.md](../GUIA_TESTING_DDD.md) - Guía completa de testing

## ✅ Checklist de Implementación

- [ ] Crear proyecto Personas.Pruebas
- [ ] Instalar paquetes NuGet
- [ ] Configurar referencias a otros proyectos
- [ ] **Tests de Domain (Prioridad 1)**
  - [ ] PersonaPruebas (10-12 tests)
  - [ ] DocumentoPruebas (3-4 tests)
- [ ] **Tests de Application (Prioridad 2)**
  - [ ] PersonaServicePruebas (6-8 tests)
- [ ] **Tests de API (Prioridad 3)**
  - [ ] PersonasControllerPruebas (4-5 tests)
- [ ] Ejecutar `RunCoverage.bat`
- [ ] Verificar cobertura >= 35%
- [ ] Capturar screenshots para sustentación

---

**Siguiente paso:** Implementar tests de Domain (PersonaPruebas.cs)
