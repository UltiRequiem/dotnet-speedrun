# Event Ticketing API

A RESTful CRUD API for managing events and tickets, built with .NET 9, Entity
Framework Core, and SQLite using Clean Architecture principles.

## Project Overview

This project demonstrates:

- Clean Architecture (Onion Architecture)
- Repository Pattern
- Dependency Injection
- Entity Framework Core with SQLite
- RESTful API design
- Best practices for separation of concerns

## Technology Stack

- **.NET 9.0** - Modern, cross-platform framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 9.0** - Object-Relational Mapper (ORM)
- **SQLite** - Lightweight, file-based database
- **C# 12** - Latest language features

## Architecture

This project follows **Clean Architecture** (also known as Onion Architecture or
Hexagonal Architecture), which provides:

- **Independence from frameworks** - Business logic doesn't depend on EF Core or
  ASP.NET
- **Testability** - Each layer can be tested independently
- **Independence from UI/Database** - Easy to swap SQLite for PostgreSQL or add
  a different UI
- **Business logic isolation** - Core domain is protected from external changes

### The Four Layers

```
┌─────────────────────────────────────────────┐
│           EventTicketing.API                │  ← Controllers, HTTP, Startup
│  (Presentation/Entry Point)                 │
└─────────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────────┐
│      EventTicketing.Infrastructure          │  ← EF Core, DbContext, Repositories
│  (Data Access & External Services)          │
└─────────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────────┐
│      EventTicketing.Application             │  ← Business Logic, Interfaces, DTOs
│  (Use Cases & Application Logic)            │
└─────────────────────────────────────────────┘
              ↓ depends on
┌─────────────────────────────────────────────┐
│        EventTicketing.Domain                │  ← Entities (Event, Ticket)
│  (Core Business Entities - NO DEPENDENCIES) │
└─────────────────────────────────────────────┘
```

**Key Principle**: Dependencies flow INWARD. The Domain layer has zero
dependencies on anything.

## Project Structure

```
EventTicketing/
├── EventTicketing.Domain/              # Core business entities
│   ├── Entities/
│   │   ├── Event.cs                    # Event entity
│   │   └── Ticket.cs                   # Ticket entity
│   └── Common/                         # Shared domain logic
│
├── EventTicketing.Application/         # Business logic & interfaces
│   ├── Interfaces/
│   │   ├── IEventRepository.cs         # Event repository contract
│   │   └── ITicketRepository.cs        # Ticket repository contract
│   ├── DTOs/                           # Data Transfer Objects
│   │   ├── EventDto.cs                 # Event API models
│   │   └── TicketDto.cs                # Ticket API models
│   └── Services/                       # Business logic services
│
├── EventTicketing.Infrastructure/      # Database & external services
│   ├── Data/
│   │   └── AppDbContext.cs             # EF Core DbContext
│   ├── Repositories/
│   │   ├── EventRepository.cs          # Event data access
│   │   └── TicketRepository.cs         # Ticket data access
│   └── Migrations/                     # Database migrations (auto-generated)
│
└── EventTicketing.API/                 # HTTP API layer
    ├── Controllers/
    │   ├── EventsController.cs         # Event CRUD endpoints
    │   └── TicketsController.cs        # Ticket CRUD endpoints
    ├── Program.cs                      # App configuration & DI setup
    └── appsettings.json                # Configuration (DB connection string)
```

## Interview Talking Points

### Why Clean Architecture?

**Problem it solves**: In traditional layered architecture, business logic gets
mixed with database and UI code. Changes ripple through the entire application.

**Solution**: Clean Architecture isolates business logic in the center (Domain).
External concerns (database, API, UI) depend on it, not vice versa.

**Benefits**:

1. **Testable**: Test business logic without spinning up a database
2. **Flexible**: Swap SQLite for PostgreSQL without changing business logic
3. **Maintainable**: Changes to database schema don't affect business rules
4. **Clear boundaries**: Each layer has a single responsibility

### Dependency Flow

```
API → Infrastructure → Application → Domain
(Web)  (Database)      (Logic)       (Entities)
```

- **Domain** knows nothing about databases, APIs, or frameworks
- **Application** defines interfaces but doesn't implement data access
- **Infrastructure** implements interfaces defined in Application
- **API** wires everything together with Dependency Injection

### Key Patterns Used

1. **Repository Pattern**: Abstracts data access logic
2. **Dependency Inversion**: High-level modules don't depend on low-level
   modules
3. **Separation of Concerns**: Each layer has a distinct responsibility
4. **Dependency Injection**: Loose coupling between components

## Next Steps

1. Define Domain entities (Event, Ticket)
2. Set up EF Core DbContext
3. Implement Repository pattern
4. Create API controllers
5. Configure Dependency Injection
6. Run migrations and test!
