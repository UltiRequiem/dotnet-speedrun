# Dependencies Guide

This document explains all NuGet packages used in the project and why we need
them.

## Installed Packages

### EventTicketing.Infrastructure

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
```

#### Microsoft.EntityFrameworkCore (9.0.0)

**What it is**: The core Entity Framework Core library - Microsoft's
Object-Relational Mapper (ORM).

**What it does**:

- Maps C# classes to database tables
- Tracks changes to objects automatically
- Generates SQL queries from C# LINQ expressions
- Manages database connections and transactions

**Why we need it**: Instead of writing raw SQL, we can work with C# objects and
EF Core handles the database operations.

**Example**:

```csharp
// Instead of: SELECT * FROM Events WHERE Id = 1
var event = await dbContext.Events.FindAsync(1);
```

**Interview answer**: "EF Core is an ORM that provides a type-safe way to
interact with databases using C# objects. It handles change tracking, query
generation, and relationship management."

---

#### Microsoft.EntityFrameworkCore.Sqlite (9.0.0)

**What it is**: SQLite database provider for Entity Framework Core.

**What it does**:

- Translates EF Core commands into SQLite-specific SQL
- Handles SQLite data types and constraints
- Manages SQLite connection strings and file paths

**Why we need it**: EF Core is database-agnostic. This package teaches it how to
talk to SQLite specifically.

**Alternatives**:

- `Microsoft.EntityFrameworkCore.SqlServer` - For SQL Server
- `Npgsql.EntityFrameworkCore.PostgreSQL` - For PostgreSQL
- `Pomelo.EntityFrameworkCore.MySql` - For MySQL

**Interview answer**: "The SQLite provider allows EF Core to generate
SQLite-specific SQL and handle SQLite's features. We can swap this for
PostgreSQL provider without changing our business logic - that's the power of
Clean Architecture."

---

#### Microsoft.EntityFrameworkCore.Design (9.0.0)

**What it is**: Design-time tools for EF Core.

**What it does**:

- Enables `dotnet ef` CLI commands
- Powers database migrations (creating/updating database schema)
- Scaffolds entities from existing databases
- Generates migration files

**Why we need it**: We need this to run commands like:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Interview answer**: "The Design package provides tooling for migrations -
versioned schema changes that let us evolve our database over time while
preserving data."

---

### EventTicketing.API

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
```

#### Why does API also need EF Core Design?

**Reason**: The `dotnet ef` commands must be run from the **startup project**
(API layer), even though the DbContext lives in Infrastructure.

**What happens**: When you run `dotnet ef migrations add`, the tool:

1. Starts from the API project (entry point)
2. Looks at Infrastructure (where DbContext is)
3. Generates migration files in Infrastructure/Migrations/

**Command structure**:

```bash
# Run from solution root
dotnet ef migrations add InitialCreate --project EventTicketing.Infrastructure --startup-project EventTicketing.API
```

---

## Dependency Graph

```
EventTicketing.API
├── Microsoft.EntityFrameworkCore.Design (9.0.0)
├── → EventTicketing.Application
└── → EventTicketing.Infrastructure
    ├── Microsoft.EntityFrameworkCore (9.0.0)
    ├── Microsoft.EntityFrameworkCore.Sqlite (9.0.0)
    ├── Microsoft.EntityFrameworkCore.Design (9.0.0)
    └── → EventTicketing.Application
        └── → EventTicketing.Domain
```

**Key principle**: Database packages only exist in Infrastructure. Domain and
Application remain database-agnostic.

---

## Version Compatibility

| .NET Version | EF Core Version | Status  |
| ------------ | --------------- | ------- |
| .NET 6       | EF Core 6.x     | LTS     |
| .NET 7       | EF Core 7.x     | EOL     |
| .NET 8       | EF Core 8.x     | LTS     |
| .NET 9       | EF Core 9.x     | Current |
| .NET 10      | EF Core 10.x    | Preview |

**Rule**: Always match your EF Core major version to your .NET major version.

---

## Common Interview Questions

### Q: Why use an ORM instead of raw SQL?

**A**:

- **Type safety**: Compiler catches errors instead of runtime failures
- **Productivity**: Less boilerplate code
- **Maintainability**: Changes to schema are centralized
- **Protection**: Prevents SQL injection automatically
- **Trade-off**: Can be slower for complex queries; you can drop down to raw SQL
  when needed

### Q: What are migrations?

**A**: Version-controlled schema changes. Each migration is a C# class that
defines:

- `Up()`: How to apply the change (add table, add column, etc.)
- `Down()`: How to reverse it

Benefits:

- Database schema is in source control
- Team members apply same changes automatically
- Can rollback changes safely
- Works across environments (dev, staging, prod)

### Q: Why SQLite for this project?

**A**:

- **Learning**: Simple setup, no server installation
- **Portability**: Database is a single file
- **Development**: Fast for local dev/testing
- **Production-ready**: Fine for small-to-medium apps
- **Limitation**: Not ideal for high-concurrency or large-scale apps

For production at scale, consider PostgreSQL or SQL Server.

### Q: What's the difference between .Add() and .AddAsync()?

**A**:

- `.Add()` is synchronous - fine for simple operations
- `.AddAsync()` is for value generators that do I/O (like HiLo sequences)
- For most cases, `.Add()` + `await SaveChangesAsync()` is sufficient
- Modern practice: Use async for all I/O operations (SaveChanges, queries)

---

## Additional Resources

- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [SQLite Documentation](https://www.sqlite.org/docs.html)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
