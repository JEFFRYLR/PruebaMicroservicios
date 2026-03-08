# Sistema de Gestion de Citas Medicas

Sistema de microservicios para la gestion integral de citas medicas, construido con .NET Framework 4.8.

## Estructura del Proyecto

CitasMedicas/
- src/
  - Personas/      (Microservicio de gestion de personas)
  - Citas/         (Microservicio de gestion de citas)
  - Recetas/       (Microservicio de gestion de recetas medicas)
- tests/           (Proyectos de pruebas unitarias)
- docs/            (Documentacion del proyecto)
- scripts/         (Scripts de automatizacion)

## Microservicios

### Personas (Puerto 4210)
- Gestion de medicos y pacientes
- CQRS + MediatR
- Base de datos: PersonasDB

### Citas (Puerto 4211)
- Agendamiento y seguimiento de citas
- CQRS + MediatR + RabbitMQ
- Base de datos: CitasDB

### Recetas (Puerto: Pendiente)
- Gestion de recetas medicas
- CQRS + MediatR
- Base de datos: RecetasDB

## Tecnologias

- .NET Framework 4.8
- ASP.NET Web API 2
- Entity Framework 6
- MediatR (CQRS)
- RabbitMQ (Mensajeria)
- SQL Server Express
- NUnit (Testing)

## Estado de Migracion

- [ ] Personas.Domain
- [ ] Personas.Application
- [ ] Personas.Infrastructure
- [ ] Personas.API
- [ ] Personas.Pruebas
- [ ] Citas.Domain
- [ ] Citas.Application
- [ ] Citas.Infrastructure
- [ ] Citas.API
- [ ] Citas.Tests
- [ ] Recetas.Domain
- [ ] Recetas.Application
- [ ] Recetas.Infrastructure
- [ ] Recetas.API
- [ ] Recetas.Tests

## Documentacion

Ver carpeta docs/ para:
- Guia de migracion
- Mapa de referencias
- Arquitectura del sistema
