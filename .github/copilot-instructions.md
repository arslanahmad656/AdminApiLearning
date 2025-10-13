# Copilot Instructions for AroAdminAPI

## Project Overview
- **AroAdminAPI** is a modular .NET solution for admin management, organized by domain-driven boundaries.
- Major components:
  - `Aro.Admin.Application.Mediator`: Application logic, CQRS patterns, and MediatR-style handlers.
  - `Aro.Admin.Application.Services`: Service interfaces for authentication, authorization, token management, logging, and more.
  - `Aro.Admin.Domain.Entities`: Core domain models (User, Role, Permission, etc.).
  - `Aro.Admin.Domain.Repository`: Repository interfaces for data access abstraction.
  - `Aro.Admin.Infrastructure.Repository`: Data access implementations.
  - `Aro.Admin.Infrastructure.Services`: Infrastructure-level services (e.g., authentication, hashing, auditing).
  - `Aro.Admin.Presentation.Api` and `Aro.Admin.Presentation.Entry`: API and entrypoint layers (not fully shown).

## Key Patterns & Conventions
- **Domain-Driven Design**: Each major folder represents a bounded context or layer.
- **CQRS/Mediator**: Application logic is separated into commands/queries and handled via mediator patterns.
- **Service Interfaces**: All business logic is abstracted behind interfaces in `Application.Services`.
- **Repository Pattern**: Data access is abstracted via interfaces in `Domain.Repository` and implemented in `Infrastructure.Repository`.
- **DTOs**: Data transfer objects are in `Application.Services/DTOs`.
- **Shared Code**: Common options and constants are in `Application.Shared` and `Domain.Shared`.

## Developer Workflows
- **Build**: Use the solution file `AroAdminAPI.sln` with standard .NET CLI:
  - `dotnet build AroAdminAPI.sln`
- **Test**: (No test projects detected in this scan. If present, use `dotnet test`.)
- **Run**: Launch via the API or entry project (e.g., `dotnet run --project Aro.Admin.Presentation.Api`).
- **Debug**: Attach to the API project or use launch configurations in your IDE.

## Integration & Dependencies
- **External Libraries**: Standard .NET libraries, MediatR/CQRS patterns, and BCrypt for hashing.
- **Cross-Component Communication**: Application layer communicates with domain and infrastructure via interfaces; infrastructure implements these interfaces.
- **No direct database or external service config found in this scan.**

## Project-Specific Notes
- **Naming**: All core types are prefixed with `Aro.Admin.` and organized by concern.
- **Extending**: Add new features by creating interfaces in `Application.Services`, implementations in `Infrastructure.Services`, and update domain/entities as needed.
- **Seeding/Migration**: See `Migration/` and `Seed/` folders in `Application.Mediator` for data seeding and migration logic.

## References
- See `README.md` for high-level project info.
- Key folders: `Aro.Admin.Application.Mediator`, `Aro.Admin.Application.Services`, `Aro.Admin.Domain.Entities`, `Aro.Admin.Domain.Repository`, `Aro.Admin.Infrastructure.Repository`, `Aro.Admin.Infrastructure.Services`.

---

_If any conventions or workflows are unclear, please provide feedback to improve these instructions._
